import { Component, Input, forwardRef, signal, computed } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-textarea',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AppTextarea),
      multi: true
    }
  ],
  templateUrl: './app-textarea.html',
  styleUrl: './app-textarea.css'
})
export class AppTextarea implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() rows: number = 4;
  @Input() readonly: boolean = false;
  @Input() required: boolean = false;
  @Input() showOptional: boolean = false;
  @Input() name: string = '';
  @Input() id: string = '';
  @Input() maxlength?: number;
  @Input() showCharCount: boolean = false;
  @Input() codeFont: boolean = false;
  @Input() invalid: boolean = false;
  @Input() hint: string = '';

  private _uniqueId = `app-textarea-${Math.random().toString(36).substring(2, 9)}`;

  get textareaId(): string {
    return this.id || this._uniqueId;
  }

  value = signal<string>('');
  disabled = signal(false);
  focused = signal(false);

  charCount = computed(() => (this.value() || '').length);

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(value: any): void {
    this.value.set(value == null ? '' : String(value));
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

  onInput(event: Event): void {
    const val = (event.target as HTMLTextAreaElement).value;
    this.value.set(val);
    this.onChange(val);
  }

  onBlur(): void {
    this.focused.set(false);
    this.onTouched();
  }

  onFocus(): void {
    this.focused.set(true);
  }
}
