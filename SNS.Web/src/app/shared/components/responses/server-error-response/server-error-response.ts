import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideHome } from '@lucide/angular';
import { ThemeChanger } from '../../../services/theme-changer';

@Component({
  selector: 'app-server-error-response',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe, LucideHome],
  templateUrl: './server-error-response.html',
  styleUrl: './server-error-response.css'
})
export class ServerErrorResponse {
  private themeChanger = inject(ThemeChanger);

  public currentTheme = this.themeChanger.currentTheme;
}
