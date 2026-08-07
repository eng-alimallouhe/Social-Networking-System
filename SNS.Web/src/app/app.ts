import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from "./identity/notifications/components/toast-container/toast-container.component";
import { LanguageService } from './shared/services/language.service';
import { ToastService } from './identity/notifications/services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastContainerComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  private toast = inject(ToastService);
  protected readonly title = signal('Social Networking System');
  private languageService = inject(LanguageService);


  constructor() {
    this.languageService.loadStoredLanguage();
  }
}
