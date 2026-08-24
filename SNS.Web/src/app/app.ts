import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from "./identity/notifications/components/toast-container/toast-container.component";
import { LanguageService } from './shared/services/language.service';
import { LineLoader } from "./shared/Loading/components/line-loader/line-loader";
import { GlobalLoaderService } from './shared/Loading/services/global-loader.service';
import { ThemeChanger } from './shared/services/theme-changer';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastContainerComponent, LineLoader],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  private globalLoader = inject(GlobalLoaderService);
  private themeChanger = inject(ThemeChanger);
  protected readonly title = signal('Social Networking System');
  private languageService = inject(LanguageService);


  isLoading = this.globalLoader.isLoading;

  constructor() {
    this.languageService.loadStoredLanguage();
    this.themeChanger.loadStoredTheme();
  }
}