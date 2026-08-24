import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideHome } from '@lucide/angular';
import { Theme, ThemeChanger } from '../../../services/theme-changer';

@Component({
  selector: 'app-not-found-response',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe, LucideHome],
  templateUrl: './not-found-response.html',
  styleUrl: './not-found-response.css'
})
export class NotFoundResponse {
  private themeChanger = inject(ThemeChanger);

  public Theme = Theme;

  public currentTheme = this.themeChanger.currentTheme;
}
