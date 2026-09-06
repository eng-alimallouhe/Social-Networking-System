import {
    Component,
    Input,
    forwardRef,
    signal,
    computed,
    inject,
    ElementRef,
    HostListener
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OverlayModule } from '@angular/cdk/overlay';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideCalendar,
    LucideChevronDown,
    LucideChevronLeft,
    LucideChevronRight,
    LucideClock
} from '@lucide/angular';

export interface CalendarDay {
    date: Date;
    dayNumber: number;
    isCurrentMonth: boolean;
    isToday: boolean;
    isSelected: boolean;
    isDisabled: boolean;
}

@Component({
    selector: 'app-date-time-picker',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        OverlayModule,
        TranslatePipe,
        LucideCalendar,
        LucideChevronDown,
        LucideChevronLeft,
        LucideChevronRight,
        LucideClock
    ],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AppDateTimePicker),
            multi: true
        }
    ],
    templateUrl: './app-date-time-picker.html',
    styleUrl: './app-date-time-picker.css'
})
export class AppDateTimePicker implements ControlValueAccessor {
    @Input() label: string = '';
    @Input() placeholder: string = 'Choose your date';
    @Input() includeTime: boolean = false;
    @Input() invalid: boolean = false;
    @Input() minDate?: Date | string;
    @Input() maxDate?: Date | string;

    isOpen = signal<boolean>(false);
    disabled = signal<boolean>(false);
    selectedDate = signal<Date | null>(null);

    // Current viewing month and year
    viewYear = signal<number>(new Date().getFullYear());
    viewMonth = signal<number>(new Date().getMonth()); // 0-11

    // Time state
    hour = signal<number>(12);
    minute = signal<number>(0);
    isPm = signal<boolean>(false);

    readonly weekDays: string[] = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'];
    readonly monthNames: string[] = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
    ];

    viewMonthTitle = computed(() => {
        return `${this.monthNames[this.viewMonth()]}, ${this.viewYear()}`;
    });

    calendarDays = computed(() => {
        const year = this.viewYear();
        const month = this.viewMonth();
        const selected = this.selectedDate();
        const today = new Date();

        const firstDayOfMonth = new Date(year, month, 1);
        const lastDayOfMonth = new Date(year, month + 1, 0);

        const startDayOfWeek = firstDayOfMonth.getDay(); // 0 (Sun) - 6 (Sat)
        const daysInMonth = lastDayOfMonth.getDate();

        const prevMonthLastDay = new Date(year, month, 0).getDate();

        const days: CalendarDay[] = [];

        // Previous month filler days
        for (let i = startDayOfWeek - 1; i >= 0; i--) {
            const date = new Date(year, month - 1, prevMonthLastDay - i);
            days.push({
                date,
                dayNumber: prevMonthLastDay - i,
                isCurrentMonth: false,
                isToday: this.isSameDay(date, today),
                isSelected: selected ? this.isSameDay(date, selected) : false,
                isDisabled: this.isDateDisabled(date)
            });
        }

        // Current month days
        for (let i = 1; i <= daysInMonth; i++) {
            const date = new Date(year, month, i);
            days.push({
                date,
                dayNumber: i,
                isCurrentMonth: true,
                isToday: this.isSameDay(date, today),
                isSelected: selected ? this.isSameDay(date, selected) : false,
                isDisabled: this.isDateDisabled(date)
            });
        }

        // Next month filler days (to complete 35 or 42 grid cells)
        const remaining = (7 - (days.length % 7)) % 7;
        for (let i = 1; i <= remaining; i++) {
            const date = new Date(year, month + 1, i);
            days.push({
                date,
                dayNumber: i,
                isCurrentMonth: false,
                isToday: this.isSameDay(date, today),
                isSelected: selected ? this.isSameDay(date, selected) : false,
                isDisabled: this.isDateDisabled(date)
            });
        }

        return days;
    });

    get displayValue(): string {
        const date = this.selectedDate();
        if (!date) return '';

        const month = this.monthNames[date.getMonth()];
        const day = date.getDate();
        const year = date.getFullYear();

        if (!this.includeTime) {
            return `${month} ${day}, ${year}`;
        }

        let h = date.getHours();
        const m = date.getMinutes().toString().padStart(2, '0');
        const ampm = h >= 12 ? 'PM' : 'AM';
        h = h % 12;
        h = h ? h : 12;

        return `${month} ${day}, ${year} ${h}:${m} ${ampm}`;
    }

    onChange: any = () => {};
    onTouched: any = () => {};

    writeValue(value: any): void {
        if (!value) {
            this.selectedDate.set(null);
            return;
        }

        const date = new Date(value);
        if (!isNaN(date.getTime())) {
            this.selectedDate.set(date);
            this.viewYear.set(date.getFullYear());
            this.viewMonth.set(date.getMonth());

            const h = date.getHours();
            this.isPm.set(h >= 12);
            this.hour.set(h % 12 || 12);
            this.minute.set(date.getMinutes());
        } else {
            this.selectedDate.set(null);
        }
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState(isDisabled: boolean): void {
        this.disabled.set(isDisabled);
    }

    toggleOpen(): void {
        if (this.disabled()) return;
        this.isOpen.update(v => !v);
    }

    close(): void {
        this.isOpen.set(false);
        this.onTouched();
    }

    prevMonth(): void {
        let m = this.viewMonth() - 1;
        let y = this.viewYear();
        if (m < 0) {
            m = 11;
            y -= 1;
        }
        this.viewMonth.set(m);
        this.viewYear.set(y);
    }

    nextMonth(): void {
        let m = this.viewMonth() + 1;
        let y = this.viewYear();
        if (m > 11) {
            m = 0;
            y += 1;
        }
        this.viewMonth.set(m);
        this.viewYear.set(y);
    }

    selectDay(day: CalendarDay): void {
        if (day.isDisabled) return;

        let hours = this.hour();
        if (this.isPm() && hours < 12) hours += 12;
        if (!this.isPm() && hours === 12) hours = 0;

        const newDate = new Date(
            day.date.getFullYear(),
            day.date.getMonth(),
            day.date.getDate(),
            this.includeTime ? hours : 0,
            this.includeTime ? this.minute() : 0,
            0
        );

        this.selectedDate.set(newDate);
        this.viewYear.set(newDate.getFullYear());
        this.viewMonth.set(newDate.getMonth());

        this.emitValue(newDate);

        if (!this.includeTime) {
            this.close();
        }
    }

    setToday(): void {
        const today = new Date();
        this.selectedDate.set(today);
        this.viewYear.set(today.getFullYear());
        this.viewMonth.set(today.getMonth());
        this.emitValue(today);
        if (!this.includeTime) {
            this.close();
        }
    }

    clear(): void {
        this.selectedDate.set(null);
        this.emitValue(null);
        this.close();
    }

    onTimeChange(): void {
        const current = this.selectedDate();
        if (!current) return;

        let hours = this.hour();
        if (this.isPm() && hours < 12) hours += 12;
        if (!this.isPm() && hours === 12) hours = 0;

        const updated = new Date(
            current.getFullYear(),
            current.getMonth(),
            current.getDate(),
            hours,
            this.minute(),
            0
        );

        this.selectedDate.set(updated);
        this.emitValue(updated);
    }

    toggleAmPm(): void {
        this.isPm.update(v => !v);
        this.onTimeChange();
    }

    private emitValue(date: Date | null): void {
        if (!date) {
            this.onChange(null);
            return;
        }

        if (!this.includeTime) {
            // YYYY-MM-DD format
            const y = date.getFullYear();
            const m = (date.getMonth() + 1).toString().padStart(2, '0');
            const d = date.getDate().toString().padStart(2, '0');
            this.onChange(`${y}-${m}-${d}`);
        } else {
            this.onChange(date.toISOString());
        }
    }

    private isSameDay(d1: Date, d2: Date): boolean {
        return (
            d1.getFullYear() === d2.getFullYear() &&
            d1.getMonth() === d2.getMonth() &&
            d1.getDate() === d2.getDate()
        );
    }

    private isDateDisabled(date: Date): boolean {
        if (this.minDate) {
            const min = new Date(this.minDate);
            min.setHours(0, 0, 0, 0);
            if (date < min) return true;
        }
        if (this.maxDate) {
            const max = new Date(this.maxDate);
            max.setHours(23, 59, 59, 999);
            if (date > max) return true;
        }
        return false;
    }
}
