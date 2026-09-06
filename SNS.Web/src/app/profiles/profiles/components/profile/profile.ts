import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideUserPlus,
    LucideUserCheck,
    LucideCalendar,
    LucideBadgeCheck,
    LucideMoreVertical,
    LucideArrowRight
} from '@lucide/angular';
import { ProfileSummaryDto } from '../../contracts/profile-summary.dto';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
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
    profile = input.required<ProfileSummaryDto>();
    profileClicked = output<string>();

    readonly defaultAvatar = 'assets/images/default-avatar.png';

    isFollowing = signal<boolean>(false);

    onAvatarError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultAvatar) {
            target.src = this.defaultAvatar;
        }
    }

    initials = computed(() => {
        const name = this.profile().fullName?.trim();
        if (!name) return '??';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    visibleSkills = computed(() => {
        const all = this.profile().skills || [];
        return all.slice(0, 5);
    });

    extraSkillsCount = computed(() => {
        const all = this.profile().skills || [];
        return all.length > 5 ? all.length - 5 : 0;
    });

    toggleFollow(): void {
        this.isFollowing.update(v => !v);
    }

    onProfileClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.profileClicked.emit(this.profile().id);
    }
}
