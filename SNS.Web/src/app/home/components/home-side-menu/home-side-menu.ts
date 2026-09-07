import { Component, inject, effect, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideX, LucideUser, LucideSettings, LucideLogOut } from '@lucide/angular';
import { HomeStateService } from '../../services/home-state.service';
import { PageService } from '../../../shared/services/page.service';
import { AppAvatar } from '../../../shared/design-system/components/app-avatar/app-avatar';
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';
import { ProfilesService } from '../../../profiles/profiles/services/profiles.service';
import { ProfileBaseDto } from '../../../profiles/profiles/contracts/profile-base.dto';
import { CommunitiesService } from '../../../content-management/communities/communities/services/communities.service';
import { CommunitySummaryDto } from '../../../content-management/communities/communities/contracts/community-summary.dto';
import { JobsService } from '../../../jobs/jobs/services/jobs.service';
import { JobSummaryDto } from '../../../jobs/jobs/contracts/job-summary.dto';
import { SkeletonLoaderComponent, SkeletonType } from '../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
  selector: 'app-home-side-menu',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    TranslatePipe,
    LucideX,
    LucideUser,
    LucideSettings,
    LucideLogOut,
    AppAvatar,
    SkeletonLoaderComponent
  ],
  templateUrl: './home-side-menu.html',
  styleUrl: './home-side-menu.css'
})
export class HomeSideMenu implements OnInit {
  public homeState = inject(HomeStateService);
  private pageService = inject(PageService);
  public authService = inject(AuthenticationService);
  private profilesService = inject(ProfilesService);
  private communitiesService = inject(CommunitiesService);
  private jobsService = inject(JobsService);

  readonly SkeletonType = SkeletonType;

  public currentProfile = signal<ProfileBaseDto | null>(null);
  public isLoadingProfile = signal<boolean>(true);

  public myCommunities = signal<CommunitySummaryDto[]>([]);
  public isLoadingMyCommunities = signal<boolean>(true);

  public suggestedCommunities = signal<CommunitySummaryDto[]>([]);
  public isLoadingSuggestedCommunities = signal<boolean>(true);

  public suggestedJobs = signal<JobSummaryDto[]>([]);
  public isLoadingSuggestedJobs = signal<boolean>(true);

  ngOnInit(): void {
    this.loadCurrentProfile();
    this.loadMyCommunities();
    this.loadSuggestedCommunities();
    this.loadSuggestedJobs();
  }

  loadCurrentProfile(): void {
    this.isLoadingProfile.set(true);
    this.profilesService.getCurrentUserProfile().subscribe({
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

  loadMyCommunities(): void {
    this.isLoadingMyCommunities.set(true);
    this.communitiesService.getMyCommunities(1, 5).subscribe({
      next: (res) => {
        if (res.isSuccess && res.value?.items) {
          this.myCommunities.set(res.value.items);
        }
        this.isLoadingMyCommunities.set(false);
      },
      error: () => {
        this.isLoadingMyCommunities.set(false);
      }
    });
  }

  loadSuggestedCommunities(): void {
    this.isLoadingSuggestedCommunities.set(true);
    this.communitiesService.getSuggestedCommunities(4).subscribe({
      next: (res) => {
        if (res.isSuccess && res.value) {
          this.suggestedCommunities.set(res.value);
        }
        this.isLoadingSuggestedCommunities.set(false);
      },
      error: () => {
        this.isLoadingSuggestedCommunities.set(false);
      }
    });
  }

  loadSuggestedJobs(): void {
    this.isLoadingSuggestedJobs.set(true);
    this.jobsService.getSuggestedJobs(3).subscribe({
      next: (res) => {
        if (res.isSuccess && res.value) {
          this.suggestedJobs.set(res.value);
        }
        this.isLoadingSuggestedJobs.set(false);
      },
      error: () => {
        this.isLoadingSuggestedJobs.set(false);
      }
    });
  }

  constructor() {
    effect(() => {
      if (this.homeState.isSideMenuOpen()) {
        this.pageService.disableScroll();
      } else {
        this.pageService.enableScroll();
      }
    });
  }

  closeMenu() {
    this.homeState.closeSideMenu();
  }
}