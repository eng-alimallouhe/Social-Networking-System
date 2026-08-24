import { Component, Input, forwardRef, ElementRef, ViewChildren, QueryList, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-code-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AppCodeInput),
      multi: true
    }
  ],
  templateUrl: './app-code-input.html',
  styleUrl: './app-code-input.css'
})
export class AppCodeInput implements ControlValueAccessor, OnInit {
  @Input() length: number = 6;
  @Input() disabled: boolean = false;
  
  digits: string[] = [];
  
  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

  onChange: any = () => { };
  onTouched: any = () => { };

  constructor() {
    this.initDigits();
  }

  ngOnInit() {
    this.initDigits();
  }

  initDigits() {
    if (this.digits.length !== this.length) {
      this.digits = Array(this.length).fill('');
    }
  }

  trackByIndex(index: number): number {
    return index;
  }

  writeValue(value: any): void {
    const strVal = value == null ? '' : String(value);
    for (let i = 0; i < this.length; i++) {
      this.digits[i] = strVal[i] || '';
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  private updateValue() {
    const val = this.digits.join('');
    this.onChange(val);
  }

  onInput(event: Event, index: number): void {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/[^0-9]/g, '');
    const cleanVal = val.length > 1 ? val.charAt(val.length - 1) : val;
    
    input.value = cleanVal;
    this.digits[index] = cleanVal;
    
    this.updateValue();

    if (cleanVal && index < this.length - 1) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number): void {
    const input = event.target as HTMLInputElement;
    if (event.key === 'Backspace') {
      if (!input.value && index > 0) {
        const prev = this.codeInputs.toArray()[index - 1];
        prev.nativeElement.value = '';
        this.digits[index - 1] = '';
        this.updateValue();
        prev.nativeElement.focus();
      } else {
        input.value = '';
        this.digits[index] = '';
        this.updateValue();
      }
    } else if (event.key === 'ArrowLeft' && index > 0) {
      this.codeInputs.toArray()[index - 1].nativeElement.focus();
    } else if (event.key === 'ArrowRight' && index < this.length - 1) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }
  
  onFocus(event: FocusEvent): void {
    (event.target as HTMLInputElement).select();
    this.onTouched();
  }

  onPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const pasted = event.clipboardData?.getData('text/plain');
    if (!pasted) return;
    
    // Gracefully filter non-numeric characters
    const nums = pasted.replace(/\D/g, '').substring(0, this.length);
    for (let i = 0; i < nums.length; i++) {
      this.digits[i] = nums[i];
    }
    
    this.updateValue();
    
    if (nums.length > 0) {
      const targetIndex = Math.min(nums.length, this.length - 1);
      setTimeout(() => {
        const inputs = this.codeInputs.toArray();
        if (inputs[targetIndex]) {
          inputs[targetIndex].nativeElement.focus();
        }
      });
    }
  }
}
