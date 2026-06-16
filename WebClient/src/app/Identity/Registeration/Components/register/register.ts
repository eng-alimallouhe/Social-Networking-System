import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-register',
  imports: [RouterLink, TranslateModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {}
