import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-account-assistance',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  templateUrl: './account-assistance.html',
  styleUrl: './account-assistance.css'
})
export class AccountAssistance { }