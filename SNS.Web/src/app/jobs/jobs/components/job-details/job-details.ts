import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideMapPin,
    LucideBanknote,
    LucideClock,
    LucideBookmark,
    LucideShare2,
    LucideSend,
    LucideCheckCircle,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideBriefcase,
    LucideFileText
} from '@lucide/angular';
import { JobsService } from '../../services/jobs.service';
import { JobDetailsDto } from '../../contracts/job-details.dto';
import { JobType } from '../../../enums/job-type.enum';
import { SalaryType } from '../../../enums/salary-type.enum';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-job-details',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideMapPin,
        LucideBanknote,
        LucideClock,
        LucideBookmark,
        LucideShare2,
        LucideSend,
        LucideCheckCircle,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideBriefcase,
        LucideFileText
    ],
    templateUrl: './job-details.html',
    styleUrl: './job-details.css'
})
export class JobDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private jobsService = inject(JobsService);

    readonly SkeletonType = SkeletonType;
    readonly JobType = JobType;
    readonly SalaryType = SalaryType;
    readonly defaultLogo = 'assets/images/default-avatar.png';

    jobId = signal<string>('');
    job = signal<JobDetailsDto | null>(null);
    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);
    isBookmarked = signal<boolean>(false);
    isApplied = signal<boolean>(false);

    initials = computed(() => {
        const name = this.job()?.company?.name?.trim();
        if (!name) return 'JB';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    isActive = computed(() => {
        const j = this.job();
        if (!j) return false;
        if (j.isActive !== undefined && j.isActive !== null) return j.isActive;
        return !j.closedAt;
    });

    responsibilitiesList = computed<string[]>(() => {
        const text = this.job()?.keyResponsibilitiesText;
        if (!text) return [];
        return text
            .split(/\r?\n/)
            .map(line => line.replace(/^[-*•]\s*/, '').trim())
            .filter(line => line.length > 0);
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('jobId') || params.get('id');
            if (id && id !== this.jobId()) {
                this.jobId.set(id);
                this.loadJob();
            }
        });
    }

    loadJob(): void {
        const id = this.jobId();
        if (!id) return;

        this.isLoading.set(true);
        this.hasError.set(false);

        this.jobsService.getJobById(id).subscribe({
            next: res => {
                this.isLoading.set(false);
                if (res?.isSuccess && res.value) {
                    this.job.set(res.value);
                } else {
                    this.hasError.set(true);
                }
            },
            error: () => {
                this.isLoading.set(false);
                this.hasError.set(true);
            }
        });
    }

    toggleBookmark(): void {
        this.isBookmarked.update(b => !b);
    }

    apply(): void {
        this.isApplied.set(true);
    }

    share(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    onLogoError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultLogo) {
            target.src = this.defaultLogo;
        }
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home/search']);
        }
    }
}
