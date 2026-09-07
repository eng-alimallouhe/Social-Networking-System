import { Component, input, output, signal, computed, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
    LucideUserPlus,
    LucideUserCheck,
    LucideCalendar,
    LucideBadgeCheck,
    LucideMoreVertical,
    LucideArrowRight
} from '@lucide/angular';
import { ProfileSummaryDto } from '../../contracts/profile-summary.dto';
import { AppAvatar } from '../../../../shared/design-system/components/app-avatar/app-avatar';
import { FollowsService } from '../../../social-graph/services/follows.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { ToastService } from '../../../../identity/notifications/services/toast.service';
import { CircleLoader } from '../../../../shared/Loading/components/circle-loader/circle-loader';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        AppAvatar,
        CircleLoader,
        LucideUserPlus,
        LucideUserCheck,
        LucideCalendar,
        LucideBadgeCheck,
        LucideMoreVertical,
        LucideArrowRight
    ],
    templateUrl: './profile.html',
    styleUrl: './profile.css'
})
export class Profile {
    private followsService = inject(FollowsService);
    private authService = inject(AuthenticationService);
    private toastService = inject(ToastService);
    private translate = inject(TranslateService);

    profile = input.required<ProfileSummaryDto>();
    profileClicked = output<string>();
    followToggled = output<boolean>();

    isFollowing = signal<boolean>(false);
    isActionLoading = signal<boolean>(false);

    currentProfileId = computed(() => this.authService.getProfileId() || this.authService.getUserId());

    isSelf = computed(() => {
        const myId = this.currentProfileId();
        const pId = this.profile()?.id;
        return !!(myId && pId && myId.toLowerCase() === pId.toLowerCase());
    });

    visibleSkills = computed(() => {
        const all = this.profile().skills || [];
        return all.slice(0, 5);
    });

    extraSkillsCount = computed(() => {
        const all = this.profile().skills || [];
        return all.length > 5 ? all.length - 5 : 0;
    });

    constructor() {
        effect(() => {
            const p = this.profile();
            if (p) {
                this.isFollowing.set(!!p.isFollowedByCurrentUser);
            }
        });
    }

    toggleFollow(): void {
        if (this.isActionLoading() || this.isSelf()) return;

        const targetProfileId = this.profile().id;
        const willFollow = !this.isFollowing();

        // Optimistic update
        this.isFollowing.set(willFollow);
        this.isActionLoading.set(true);

        const request$ = willFollow
            ? this.followsService.followProfile(targetProfileId)
            : this.followsService.unfollowProfile(targetProfileId);

        request$.subscribe({
            next: res => {
                this.isActionLoading.set(false);
                if (res?.isSuccess) {
                    this.followToggled.emit(willFollow);
                } else {
                    // Rollback
                    this.isFollowing.set(!willFollow);
                    this.toastService.error(
                        this.translate.instant('Common.Error') || 'Error',
                        this.translate.instant('Profile.Follow_Error') || 'Failed to update follow status.'
                    );
                }
            },
            error: () => {
                this.isActionLoading.set(false);
                // Rollback
                this.isFollowing.set(!willFollow);
                this.toastService.error(
                    this.translate.instant('Common.Error') || 'Error',
                    'An error occurred while updating follow status.'
                );
            }
        });
    }

    onProfileClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.profileClicked.emit(this.profile().id);
    }
}
