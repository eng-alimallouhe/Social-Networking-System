import { Component, Input, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AppInput),
      multi: true
    }
  ],
  templateUrl: './app-input.html',
  styleUrl: './app-input.css'
})
export class AppInput implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: 'text' | 'password' | 'email' | 'number' | 'search' | 'tel' | 'url' = 'text';
  @Input() readonly: boolean = false;
  @Input() required: boolean = false;
  @Input() autocomplete: string = 'off';
  @Input() name: string = '';
  @Input() id: string = '';
  @Input() maxlength?: number;
  @Input() invalid: boolean = false;
  @Input() hint: string = '';

  private _uniqueId = `app-input-${Math.random().toString(36).substring(2, 9)}`;
  
  get inputId(): string {
    return this.id || this._uniqueId;
  }

  value = signal<any>('');
  disabled = signal(false);
  focused = signal(false);

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(value: any): void {
    // Treat null/undefined as empty string for inputs
    this.value.set(value == null ? '' : value);
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
    const val = (event.target as HTMLInputElement).value;
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
