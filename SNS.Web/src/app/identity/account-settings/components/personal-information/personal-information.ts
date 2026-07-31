import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { LucideChevronLeft, LucideUserRoundKey, LucideMail, LucideSquareArrowOutUpRight, LucideEarth, LucideSquareAsterisk, LucideMapPinned, LucideMapPinPen, LucideCloudSync, LucideSquareUserRound } from '@lucide/angular';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, filter, finalize, map, mergeMap, timer } from 'rxjs';
import { PersonalInformationDto } from '../../../users/user-management/contracts/user-personal-informations.dto';
import { UserManagementService } from '../../../users/user-management/services/user-management.service';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';
import { CircleLoader } from "../../../../shared/components/loaders/circle-loader/circle-loader";
import { SettingsLayout } from '../settings-layout/settings-layout';
import { LoadingSettingsService } from '../../services/loading-settings.service';

@Component({
  selector: 'app-personal-information',
  imports: [
    RouterLink,
    TranslatePipe,
    AsyncPipe,
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
    RouterOutlet,
    CircleLoader
  ],
  templateUrl: './personal-information.html',
  styleUrl: './personal-information.css',
})
export class PersonalInformation implements OnInit {
  private userManagementService = inject(UserManagementService);
  public personalInformation$!: Observable<PersonalInformationDto | null>;
  private router = inject(Router);
  private loadingService = inject(LoadingSettingsService);

  readonly isPersonalInfoRootRoute = toSignal(
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.router.url === '/account-settings/personal-information')
    ),
    {
      initialValue: this.router.url === '/account-settings/personal-information'
    }
  );


  ngOnInit(): void {
    if (this.isPersonalInfoRootRoute()) {
      setTimeout(() => this.loadingService.show());
    }
    const started = Date.now();
    this.personalInformation$ = this.userManagementService.getPersonalInformation()
      .pipe(
        mergeMap(result => {
          const elapsed = Date.now() - started;
          const remaining = Math.max(0, 1000 - elapsed);

          return timer(remaining).pipe(
            map(() => result)
          );
        }),
        finalize(() => {
          setTimeout(() => this.loadingService.hide());
        })
      );
  }

  navigateToUsernameChange(username: string) {
    this.router.navigate(['/account-settings/personal-information/change-username'], {
      state: { username: username }
    });
  }
}
