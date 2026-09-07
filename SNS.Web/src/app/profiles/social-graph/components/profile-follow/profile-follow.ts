import { Component, input, output, signal, computed, inject, effect } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideCalendar,
    LucideUserPlus,
    LucideUserCheck,
    LucideArrowRight,
    LucideBadgeCheck
} from '@lucide/angular';
import { ProfileFollowDto } from '../../contracts/profile-follow.dto';
import { FollowsService } from '../../services/follows.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { AppAvatar } from '../../../../shared/design-system/components/app-avatar/app-avatar';

@Component({
    selector: 'app-profile-follow',
    standalone: true,
    imports: [
        CommonModule,
        DatePipe,
        TranslatePipe,
        AppAvatar,
        LucideCalendar,
        LucideUserPlus,
        LucideUserCheck,
        LucideArrowRight,
        LucideBadgeCheck
    ],
    templateUrl: './profile-follow.html',
    styleUrl: './profile-follow.css'
})
export class ProfileFollow {
    private followsService = inject(FollowsService);
    private authService = inject(AuthenticationService);

    follow = input.required<ProfileFollowDto>();
    initialIsFollowing = input<boolean>(false);
    showAction = input<boolean>(true);

    profileClicked = output<string>();
    followToggled = output<{ profileId: string; isFollowing: boolean }>();

    isFollowing = signal<boolean>(false);
    isActionLoading = signal<boolean>(false);

    currentProfileId = computed(() => {
        return this.authService.getProfileId() || this.authService.getUserId();
    });

    isSelf = computed(() => {
        const myId = this.currentProfileId();
        const theirId = this.follow()?.profileId;
        return !!(myId && theirId && myId.toLowerCase() === String(theirId).toLowerCase());
    });

    constructor() {
        effect(() => {
            this.isFollowing.set(this.initialIsFollowing());
        });
    }

    onProfileClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.profileClicked.emit(this.follow().profileId);
    }

    toggleFollow(): void {
        if (this.isActionLoading() || this.isSelf()) return;

        const profileId = this.follow().profileId;
        const nextState = !this.isFollowing();

        // Optimistic update
        this.isFollowing.set(nextState);
        this.isActionLoading.set(true);

        const request$ = nextState
            ? this.followsService.followProfile(profileId)
            : this.followsService.unfollowProfile(profileId);

        request$.subscribe({
            next: res => {
                this.isActionLoading.set(false);
                if (res?.isSuccess) {
                    this.followToggled.emit({ profileId, isFollowing: nextState });
                } else {
                    // Rollback on non-success result
                    this.isFollowing.set(!nextState);
                }
            },
            error: () => {
                this.isActionLoading.set(false);
                // Rollback on network/server error
                this.isFollowing.set(!nextState);
            }
        });
    }
}
