import {
    Component,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import {
    LucideBriefcase
} from '@lucide/angular';

import { Job } from '../job/job';
import { JobsService } from '../../services/jobs.service';
import { JobSummaryDto } from '../../contracts/job-summary.dto';

import {
    SkeletonLoaderComponent,
    SkeletonType
} from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-suggested-jobs',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        LucideBriefcase,
        Job,
        SkeletonLoaderComponent
    ],
    templateUrl: './suggested-jobs.html',
    styleUrl: './suggested-jobs.css'
})
export class SuggestedJobs {

    private readonly jobsService = inject(JobsService);

    public loaderType = SkeletonType;

    readonly jobs = signal<JobSummaryDto[]>([]);

    readonly isLoading = signal(true);

    readonly skeletonItems = Array.from({ length: 5 });

    ngOnInit(): void {
        this.loadSuggestedJobs();
    }

    private loadSuggestedJobs(): void {

        this.isLoading.set(true);

        this.jobsService
            .getSuggestedJobs(10)
            .subscribe({
                next: response => {

                    if (response.isSuccess && response.value) {
                        this.jobs.set(response.value);
                    }

                    this.isLoading.set(false);
                },

                error: () => {
                    this.isLoading.set(false);
                }
            });
    }
}