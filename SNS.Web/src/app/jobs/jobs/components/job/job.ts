import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideBookmark,
    LucideMoreVertical,
    LucideMapPin,
    LucideBanknote,
    LucideClock,
    LucideUsers
} from '@lucide/angular';
import { JobSummaryDto } from '../../contracts/job-summary.dto';

@Component({
    selector: 'app-job',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        LucideBookmark,
        LucideMoreVertical,
        LucideMapPin,
        LucideBanknote,
        LucideClock,
        LucideUsers
    ],
    templateUrl: './job.html',
    styleUrl: './job.css'
})
export class Job {
    job = input.required<JobSummaryDto>();
    jobClicked = output<string>();

    readonly defaultCompanyLogo = 'assets/images/default-avatar.png';

    isBookmarked = signal<boolean>(false);

    isActive = computed(() => {
        const j = this.job();
        if (j.isActive !== undefined && j.isActive !== null) return j.isActive;
        if (j.closedAt) return false;
        if (j.isClosed !== undefined && j.isClosed !== null) return !j.isClosed;
        return true;
    });

    initials = computed(() => {
        const name = this.job().companyName?.trim();
        if (!name) return 'JB';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    visibleSkills = computed(() => {
        const skills = this.job().skills;
        if (!skills || !Array.isArray(skills)) return [];
        return skills.slice(0, 4);
    });

    extraSkillsCount = computed(() => {
        const skills = this.job().skills;
        if (!skills || !Array.isArray(skills)) return 0;
        return Math.max(0, skills.length - 4);
    });

    onLogoError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultCompanyLogo) {
            target.src = this.defaultCompanyLogo;
        }
    }

    toggleBookmark(): void {
        this.isBookmarked.update(b => !b);
    }

    onJobClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.jobClicked.emit(this.job().id);
    }
}
