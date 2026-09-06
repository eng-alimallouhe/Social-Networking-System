import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideGlobe,
    LucideLock,
    LucideCalendar,
    LucideUsers,
    LucideShield,
    LucideMoreVertical,
    LucideArrowRight,
    LucideUserPlus,
    LucideCheck,
    LucideBadgeCheck
} from '@lucide/angular';
import { CommunitySummaryDto } from '../../contracts/community-summary.dto';
import { CommunityType } from '../../../../../shared/contracts/community-type';

@Component({
    selector: 'app-community',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        LucideGlobe,
        LucideLock,
        LucideCalendar,
        LucideUsers,
        LucideShield,
        LucideMoreVertical,
        LucideArrowRight,
        LucideUserPlus,
        LucideCheck,
        LucideBadgeCheck
    ],
    templateUrl: './community.html',
    styleUrl: './community.css'
})
export class Community {
    community = input.required<CommunitySummaryDto>();
    communityClicked = output<string>();

    readonly CommunityType = CommunityType;
    isMember = signal<boolean>(false);

    initials = computed(() => {
        const name = this.community().name?.trim();
        if (!name) return 'CO';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    isPublic = computed(() => {
        return this.community().type === CommunityType.Public;
    });

    toggleMembership(): void {
        this.isMember.update(v => !v);
    }

    onCommunityClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.communityClicked.emit(this.community().id);
    }
}
