import { Component, computed, Inject, inject, OnInit, signal } from '@angular/core';
import { UserManagementService } from '../../../users/user-management/services/user-management.service';
import { every, filter, map, mergeMap, Observable, timer } from 'rxjs';
import { Result } from '../../../../shared/contracts/result';
import { PersonalInformationDto } from '../../../users/user-management/contracts/user-personal-informations.dto';
import { LucideChevronsLeft, LucideChevronLeft, LucideSquareUser, LucideUserRoundKey, LucideMail, LucideGlobe, LucideEarth, LucideSquareAsterisk, LucideMapPinned, LucideMapPinPen, LucideCloudSync, LucideSquareUserRound, LucideSquareArrowOutUpRight } from '@lucide/angular';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AsyncPipe } from '@angular/common';
import { CircleLoaderComponent } from '../../../../shared/components/loaders/circle-loader/circle-loader.component';
import { LocalDatePipe } from "../../../../shared/pipes/local-date.pipe";
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-personal-information',
  imports: [
    RouterLink,
    TranslatePipe,
    AsyncPipe,
    CircleLoaderComponent,
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
  templateUrl: './personal-information.component.html',
  styleUrl: './personal-information.component.css',
})
export class PersonalInformationComponent implements OnInit {
  private userManagementService = inject(UserManagementService);
  public personalInformation$!: Observable<PersonalInformationDto | null>;
  private router = inject(Router);

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
    console.log("Personal Information Component is loaded from Oninit");
    const started = Date.now();
    this.personalInformation$ = this.userManagementService.getPersonalInformation()
      .pipe(
        mergeMap(result => {
          const elapsed = Date.now() - started;
          const remaining = Math.max(0, 1000 - elapsed);

          return timer(remaining).pipe(
            map(() => result)
          );
        })
      );
  }


  navigateToUsernameChange(username: string) {
    this.router.navigate(['/account-settings/personal-information/change-username'], {
      state: { username: username }
    });
  }
}
