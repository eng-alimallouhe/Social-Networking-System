import { Component, Input, forwardRef, HostListener, signal, ElementRef, inject, computed } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OverlayModule } from '@angular/cdk/overlay'; 

export interface SelectOption {
  value: any;
  label: string;
}

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [CommonModule, OverlayModule], 
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AppSelect),
      multi: true
    }
  ],
  templateUrl: './app-select.html',
  styleUrl: './app-select.css'
})
export class AppSelect implements ControlValueAccessor {
  private elementRef = inject(ElementRef);

  @Input() label: string = '';
  @Input() placeholder: string = 'Select an option';
  @Input() options: SelectOption[] = [];
  @Input() invalid: boolean = false;
  @Input() enableSearch: boolean = false;
  @Input() searchPlaceholder: string = 'Search...';
  @Input() isLoading: boolean = false;

  selectedValue = signal<any>(null);
  isOpen = signal(false);
  focusedIndex = signal(-1);
  disabled = signal(false);
  searchQuery = signal('');

  filteredOptions = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    if (!q || !this.enableSearch) {
      return this.options;
    }
    return this.options.filter(opt => opt.label.toLowerCase().includes(q));
  });

  onChange: any = () => { };
  onTouched: any = () => { };

  get selectedLabel(): string {
    const matched = this.options.find(opt => opt.value === this.selectedValue());
    return matched ? matched.label : this.placeholder;
  }

  writeValue(value: any): void {
    this.selectedValue.set(value);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  toggleOpen(): void {
    if (this.disabled()) return;
    this.isOpen.set(!this.isOpen());
    if (this.isOpen()) {
      this.searchQuery.set('');
      const idx = this.filteredOptions().findIndex(opt => opt.value === this.selectedValue());
      this.focusedIndex.set(idx !== -1 ? idx : 0);
    }
  }

  selectOption(option: SelectOption): void {
    if (this.disabled()) return;
    this.selectedValue.set(option.value);
    this.onChange(option.value);
    this.onTouched();
    this.isOpen.set(false);
  }

  // 👈 تم حذف HostListener الخاص بالنقرات الخارجية (onClickOutside) لأن الـ CDK سيتكفل بها

  @HostListener('keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent): void {
    if (this.disabled()) return;

    if (event.key === ' ' || event.key === 'Enter') {
      event.preventDefault();
      if (!this.isOpen()) {
        this.toggleOpen();
      } else if (this.focusedIndex() >= 0 && this.focusedIndex() < this.filteredOptions().length) {
        this.selectOption(this.filteredOptions()[this.focusedIndex()]);
      }
    } else if (event.key === 'Escape') {
      this.isOpen.set(false);
    } else if (event.key === 'ArrowDown') {
      event.preventDefault();
      if (!this.isOpen()) {
        this.toggleOpen();
      } else {
        const next = (this.focusedIndex() + 1) % this.filteredOptions().length;
        this.focusedIndex.set(next);
      }
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      if (!this.isOpen()) {
        this.toggleOpen();
      } else {
        const prev = (this.focusedIndex() - 1 + this.filteredOptions().length) % this.filteredOptions().length;
        this.focusedIndex.set(prev);
      }
    }
  }
}