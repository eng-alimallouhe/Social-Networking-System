import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from "./shared/components/toast/toast-container/toast-container.component";
import { ProfileService } from './profiles/profiles/services/profile.service';
import { TranslateService } from '@ngx-translate/core';
import { single } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastContainerComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly profileService = inject(ProfileService);
  public readonly translateService = inject(TranslateService);
  langReady = signal<boolean>(false);
  protected readonly title = signal('SyrianDevs');
  log = ''
  ngOnInit(): void {
    this.profileService.loadProfile();
  }
}
