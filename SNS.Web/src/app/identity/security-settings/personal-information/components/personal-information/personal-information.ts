import { Component, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { LucideChevronLeft, LucideUserRoundKey, LucideMail, LucideSquareArrowOutUpRight, LucideEarth, LucideSquareAsterisk, LucideMapPinned, LucideMapPinPen, LucideCloudSync, LucideSquareUserRound } from '@lucide/angular';
import { TranslatePipe } from '@ngx-translate/core';
import { filter, map, of, finalize } from 'rxjs';
import { PersonalInformationDto } from '../../../../users/user-management/contracts/user-personal-informations.dto';
import { UserManagementService } from '../../../../users/user-management/services/user-management.service';
import { LocalDatePipe } from '../../../../../shared/pipes/local-date.pipe';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';

@Component({
  selector: 'app-personal-information',
  imports: [
    RouterLink,
    TranslatePipe,
    LucideChevronLeft,
    LucideUserRoundKey,
    LucideMail,
    LucideSquareArrowOutUpRight,
    LucideEarth,
    LucideSquareAsterisk,
    LucideMapPinned,
    LucideMapPinPen,
    LucideCloudSync,
    LucideSquareUserRound,
    LocalDatePipe,
    RouterOutlet
  ],
  templateUrl: './personal-information.html',
  styleUrl: './personal-information.css',
})
export class PersonalInformation {
  private userManagementService = inject(UserManagementService);
  private router = inject(Router);
  private loadingService = inject(LoadingSettingsService);
  private personalInformationCach: PersonalInformationDto | null = null;

  readonly isPersonalInfoRootRoute = toSignal(
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.router.url === '/account-settings/personal-information')
    ),
    {
      initialValue: this.router.url === '/account-settings/personal-information'
    }
  );

  personalInfoResource = rxResource({
    params: () => ({
      isRoot: this.isPersonalInfoRootRoute()
    }),
    stream: ({ params }) => {
      if (!params.isRoot || this.personalInformationCach !== null) {
        return of(this.personalInformationCach);
      }

      this.loadingService.show();
      return this.userManagementService.getPersonalInformation().pipe(
        map(result => {
          this.personalInformationCach = result;
          return result;
        }),
        finalize(() => {
          this.loadingService.hide();
        })
      );
    }
  });

  navigateToUsernameChange(username: string) {
    this.router.navigate(['/account-settings/personal-information/change-username'], {
      state: { username: username }
    });
  }

  navigateToLanguageChange() {
    this.router.navigate(['/account-settings/personal-information/change-language']);
  }

  navigateToEmailCgange() {
    this.router.navigate(['/account-settings/personal-information/change-email']);
  }
}