import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-recover-account-by-security-code',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  templateUrl: './recover-account-by-security-code.html',
  styleUrl: './recover-account-by-security-code.css',
})
export class RecoverAccountBySecurityCode {

  // وظيفة لتنسيق الكود المكون من 16 رقم تلقائياً
  formatCode(event: any) {
    let input = event.target.value.replace(/[^a-zA-Z0-9]/g, ''); // إزالة أي رموز غير الأرقام والحروف
    let formatted = '';
    
    for (let i = 0; i < input.length; i++) {
      if (i > 0 && i % 4 === 0) {
        formatted += '-';
      }
      formatted += input[i];
    }
    
    event.target.value = formatted.toUpperCase();
  }
}