import { Component, signal, HostListener, OnInit, inject, Signal, Renderer2, DOCUMENT } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideMenu, LucideUsersRound, LucideBriefcase, LucideMessageSquareCode, LucideSettings, LucideSquareUser, LucideFolderKanban, LucideSearch, LucideHouse, LucideHome, LucideX } from '@lucide/angular';
import { ToastContainerComponent } from "../toast/toast-container/toast-container.component";
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';
import { ToastService } from '../../services/toast.service';


@Component({
  selector: 'app-navbar',
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    TranslatePipe,
    LucideMessageSquareCode,
    LucideSquareUser,
    LucideFolderKanban,
    LucideSearch,
    LucideHouse,
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  private authenticationService = inject(AuthenticationService);
  private renderer = inject(Renderer2);
  private document = inject(DOCUMENT);
  private toastService = inject(ToastService);

  public isAuthenticated: Signal<boolean> = signal(false);

  ngOnInit() {
    this.isAuthenticated = this.authenticationService.isAuthenticated;
  }
}
