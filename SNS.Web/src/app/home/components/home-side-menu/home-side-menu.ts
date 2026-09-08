import {
  Component,
  effect,
  inject,
  OnInit,
  signal
} from '@angular/core';

import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

import {
  LucideBriefcase,
  LucideLogOut,
  LucideSettings,
  LucideUser,
  LucideUsers,
  LucideX
} from '@lucide/angular';

import { HomeStateService } from '../../services/home-state.service';
import { PageService } from '../../../shared/services/page.service';

import { AppAvatar } from '../../../shared/design-system/components/app-avatar/app-avatar';

import { AuthenticationService } from '../../../identity/shared/services/authentication.service';

import { ProfilesService } from '../../../profiles/profiles/services/profiles.service';
import { ProfileBaseDto } from '../../../profiles/profiles/contracts/profile-base.dto';
import { SessionManagementService } from '../../../identity/account-settings/security-sessions/session-management/services/session-management.service';
import { GlobalLoaderService } from '../../../shared/Loading/services/global-loader.service';
import { finalize } from 'rxjs';
import { load } from '@fingerprintjs/fingerprintjs';
import { routes } from '../../../app.routes';

@Component({
  selector: 'app-home-side-menu',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    TranslatePipe,

    LucideX,
    LucideUser,
    LucideSettings,
    LucideLogOut,
    LucideUsers,
    LucideBriefcase,

    AppAvatar
  ],
  templateUrl: './home-side-menu.html',
  styleUrl: './home-side-menu.css'
})
export class HomeSideMenu implements OnInit {

  public readonly homeState = inject(HomeStateService);

  private readonly pageService = inject(PageService);

  public readonly authService =
    inject(SessionManagementService);

  public readonly authenticationService =
    inject(AuthenticationService);

  public readonly loaderService =
    inject(GlobalLoaderService);

  private readonly profilesService =
    inject(ProfilesService);

  private readonly router =
    inject(Router);

  public readonly currentProfile =
    signal<ProfileBaseDto | null>(null);

  public readonly isLoadingProfile =
    signal<boolean>(true);

  constructor() {

    effect(() => {

      if (this.homeState.isSideMenuOpen()) {
        this.pageService.disableScroll();
      } else {
        this.pageService.enableScroll();
      }

    });

  }

  ngOnInit(): void {
    this.loadCurrentProfile();
  }

  private loadCurrentProfile(): void {

    this.isLoadingProfile.set(true);

    this.profilesService
      .getCurrentUserProfile()
      .subscribe({

        next: (res) => {

          if (res.isSuccess && res.value) {
            this.currentProfile.set(res.value);
          }

          this.isLoadingProfile.set(false);
        },

        error: () => {
          this.isLoadingProfile.set(false);
        }

      });
  }

  closeMenu(): void {
    this.homeState.closeSideMenu();
  }

  logout(): void {
    this.loaderService.show();
    this.authService.logout()
      .pipe(finalize(() => {
        this.loaderService.hide();
      }))
      .subscribe({
        next: () => {
          this.router.navigate(['/demo/role-switcher'])
        }
      });
  }
}