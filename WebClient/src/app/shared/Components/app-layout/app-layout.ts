import { Component, signal, HostListener, OnInit, inject, Signal, Renderer2, DOCUMENT } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { TokenService } from '../../../Identity/Shared/Services/token.service';
import { ProfileService } from '../../../social-graph/services/profile.service';
import { ProfileBaseDto } from '../../../social-graph/dtos/profile-base-dto.dto';
import { LucideMenu, LucideUsersRound, LucideBriefcase, LucideMessageSquareCode, LucideSettings, LucideSquareUser, LucideFolderKanban, LucideSearch, LucideHouse, LucideHome, LucideX } from '@lucide/angular';
import { AuthenticationService } from '../../../Identity/Authentication/Services/authentication.service';
import { ToastService } from '../../services/toast.service';
import { ToastContainerComponent } from "../toast/toast-container/toast-container.component";

@Component({
  selector: 'app-app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    LucideMenu,
    TranslatePipe,
    LucideUsersRound,
    LucideBriefcase,
    LucideMessageSquareCode,
    LucideSettings,
    LucideSquareUser,
    LucideFolderKanban,
    LucideSearch,
    LucideHouse,
    LucideX,
    ToastContainerComponent
],
  templateUrl: './app-layout.html',
  styleUrl: './app-layout.css'
})
export class AppLayout {
  private authenticationService = inject(AuthenticationService);
  private profileService = inject(ProfileService);
  private renderer = inject(Renderer2);
  private document = inject(DOCUMENT);
  private toastService = inject(ToastService);
  

  public isAuthenticated: Signal<boolean> = signal(false);

  public profile: Signal<ProfileBaseDto | null> = this.profileService.userProfile;
  public isSidebarOpen = signal(false);


  ngOnInit() {
  }


  toggleSidebar() {
    this.isSidebarOpen.update(value => !value);
    if (this.isSidebarOpen()) {
      this.renderer.addClass(this.document.body, 'no-scroll');
    } else {
      this.renderer.removeClass(this.document.body, 'no-scroll');
    }
  }

  toggleSidebarOnClick() {
    if (this.isSidebarOpen()) {
      this.toggleSidebar();
    }
  }
}