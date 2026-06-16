import { Component, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-activate-account',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './activate-account.html',
  styleUrl: './activate-account.css'
})
export class ActivateAccount {
  @ViewChildren('otpInput') inputs!: QueryList<ElementRef<HTMLInputElement>>;

  otpFields = [0, 1, 2, 3, 4, 5];

  onFocus(event: FocusEvent) {
    const input = event.target as HTMLInputElement;
    input.select();
  }

  onInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    if (value.length > 0) {
      const lastChar = value.slice(-1);
      input.value = lastChar;
      value = lastChar;
    }
    
    if (!/^\d$/.test(value)) {
      input.value = '';
      return;
    }

    if (value && index < 5) {
      this.focusInput(index + 1);
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;

    if (event.key === 'Backspace') {
      if (!input.value && index > 0) {
        event.preventDefault();
        const prevInput = this.getInputElement(index - 1);
        prevInput.value = '';
        prevInput.focus();
      }
    }
    else if (event.key === 'ArrowLeft' && index > 0) {
      event.preventDefault();
      this.focusInput(index - 1);
    }
    else if (event.key === 'ArrowRight' && index < 5) {
      event.preventDefault();
      this.focusInput(index + 1);
    }
  }

  onPaste(event: ClipboardEvent) {
    event.preventDefault();
    const pasteData = event.clipboardData?.getData('text')
      .trim()
      .replace(/\D/g, '')
      .slice(0, 6);

    if (!pasteData) return;

    const inputArray = this.inputs.toArray();
    pasteData.split('').forEach((digit, i) => {
      if (inputArray[i]) {
        inputArray[i].nativeElement.value = digit;
      }
    });

    const nextIndex = pasteData.length < 6 ? pasteData.length : 5;
    this.focusInput(nextIndex);
  }

  private focusInput(index: number) {
    this.getInputElement(index)?.focus();
  }

  private getInputElement(index: number): HTMLInputElement {
    return this.inputs.toArray()[index].nativeElement;
  }
}