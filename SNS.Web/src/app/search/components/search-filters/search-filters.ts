import { Component, input, output, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SearchCategory } from '../../contracts/search.dto';
import { ProjectStatus } from '../../../projects/enums/project-status.enum';
import { CommunityType } from '../../../shared/contracts/community-type';
import { JobType } from '../../../jobs/enums/job-type.enum';
import { SalaryType } from '../../../jobs/enums/salary-type.enum';
import { DifficultyLevel } from '../../../discussions/shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../../discussions/problems/enums/problem-status.enum';
import { AppInput } from '../../../shared/design-system/components/app-input/app-input';
import { AppSelect, SelectOption } from '../../../shared/design-system/components/app-select/app-select';
import { AppDateTimePicker } from '../../../shared/design-system/components/app-date-time-picker/app-date-time-picker';

@Component({
    selector: 'app-search-filters',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        TranslatePipe,
        AppInput,
        AppSelect,
        AppDateTimePicker
    ],
    templateUrl: './search-filters.html',
    styleUrl: './search-filters.css'
})
export class SearchFilters {
    private translate = inject(TranslateService);
    private langChange = toSignal(this.translate.onLangChange);

    category = input<SearchCategory | null>(null);

    filterChange = output<any>();
    resetFilters = output<void>();

    // Posts Filters
    tagsInput = signal<string>('');
    topicsInput = signal<string>('');
    postDateFrom = signal<string>('');
    postDateTo = signal<string>('');

    // Profiles Filters
    profileSkillsInput = signal<string>('');

    // Projects Filters
    projectStatus = signal<number | null>(null);
    projectSkillsInput = signal<string>('');
    projectMinContributors = signal<number | null>(null);
    projectMaxContributors = signal<number | null>(null);
    projectMinRate = signal<number | null>(null);
    projectDateFrom = signal<string>('');
    projectDateTo = signal<string>('');

    // Communities Filters
    communityType = signal<number | null>(null);

    // Jobs Filters
    jobType = signal<number | null>(null);
    jobSalaryType = signal<number | null>(null);
    jobMinSalary = signal<number | null>(null);
    jobMaxSalary = signal<number | null>(null);
    jobDateFrom = signal<string>('');
    jobDateTo = signal<string>('');

    // Problems Filters
    problemLevel = signal<number | null>(null);
    problemStatus = signal<number | null>(null);
    problemDateFrom = signal<string>('');
    problemDateTo = signal<string>('');

    // Select Options with Translations
    projectStatusOptions = computed<SelectOption[]>(() => {
        this.langChange(); // track lang changes
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Statuses') },
            { value: ProjectStatus.Draft, label: this.translate.instant('Search.Filter_Labels.Draft') },
            { value: ProjectStatus.Ongoing, label: this.translate.instant('Search.Filter_Labels.Ongoing') },
            { value: ProjectStatus.Completed, label: this.translate.instant('Search.Filter_Labels.Completed') },
            { value: ProjectStatus.Published, label: this.translate.instant('Search.Filter_Labels.Published') },
            { value: ProjectStatus.Archived, label: this.translate.instant('Search.Filter_Labels.Archived') }
        ];
    });

    communityTypeOptions = computed<SelectOption[]>(() => {
        this.langChange();
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Types') },
            { value: CommunityType.Public, label: this.translate.instant('Community.Type.Public') },
            { value: CommunityType.Private, label: this.translate.instant('Community.Type.Private') }
        ];
    });

    jobTypeOptions = computed<SelectOption[]>(() => {
        this.langChange();
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Types') },
            { value: JobType.FullTime, label: this.translate.instant('Job.Type.FullTime') },
            { value: JobType.PartTime, label: this.translate.instant('Job.Type.PartTime') },
            { value: JobType.Contract, label: this.translate.instant('Job.Type.Contract') },
            { value: JobType.Internship, label: this.translate.instant('Job.Type.Internship') },
            { value: JobType.Remote, label: this.translate.instant('Job.Type.Remote') }
        ];
    });

    jobSalaryTypeOptions = computed<SelectOption[]>(() => {
        this.langChange();
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Salary_Types') },
            { value: SalaryType.Monthly, label: this.translate.instant('Job.SalaryType.Monthly') },
            { value: SalaryType.Yearly, label: this.translate.instant('Job.SalaryType.Yearly') },
            { value: SalaryType.Hourly, label: this.translate.instant('Job.SalaryType.Hourly') },
            { value: SalaryType.Negotiable, label: this.translate.instant('Job.SalaryType.Negotiable') }
        ];
    });

    problemLevelOptions = computed<SelectOption[]>(() => {
        this.langChange();
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Levels') },
            { value: DifficultyLevel.Easy, label: this.translate.instant('Problem.Level.Easy') },
            { value: DifficultyLevel.Medium, label: this.translate.instant('Problem.Level.Medium') },
            { value: DifficultyLevel.Hard, label: this.translate.instant('Problem.Level.Hard') }
        ];
    });

    problemStatusOptions = computed<SelectOption[]>(() => {
        this.langChange();
        return [
            { value: null, label: this.translate.instant('Search.Filter_Labels.All_Statuses') },
            { value: ProblemStatus.Open, label: this.translate.instant('Problem.Status.Open') },
            { value: ProblemStatus.Solved, label: this.translate.instant('Problem.Status.Solved') },
            { value: ProblemStatus.Closed, label: this.translate.instant('Problem.Status.Closed') }
        ];
    });

    apply(): void {
        const cat = this.category();
        let payload: any = {};

        if (cat === 'Posts') {
            payload = {
                tags: this.parseArray(this.tagsInput()),
                topics: this.parseArray(this.topicsInput()),
                minCreatedAt: this.postDateFrom() || null,
                maxCreatedAt: this.postDateTo() || null
            };
        } else if (cat === 'People') {
            payload = {
                requiredSkills: this.parseArray(this.profileSkillsInput())
            };
        } else if (cat === 'Projects') {
            payload = {
                status: this.projectStatus() !== null ? this.projectStatus() : null,
                requiredSkills: this.parseArray(this.projectSkillsInput()),
                minContributors: this.projectMinContributors(),
                maxContributors: this.projectMaxContributors(),
                minRate: this.projectMinRate(),
                minCreatedAt: this.projectDateFrom() || null,
                maxCreatedAt: this.projectDateTo() || null
            };
        } else if (cat === 'Communities') {
            payload = {
                type: this.communityType() !== null ? this.communityType() : null
            };
        } else if (cat === 'Jobs') {
            payload = {
                type: this.jobType() !== null ? this.jobType() : null,
                salaryType: this.jobSalaryType() !== null ? this.jobSalaryType() : null,
                minSalary: this.jobMinSalary(),
                maxSalary: this.jobMaxSalary(),
                minCreatedAt: this.jobDateFrom() || null,
                maxCreatedAt: this.jobDateTo() || null
            };
        } else if (cat === 'Problems') {
            payload = {
                level: this.problemLevel() !== null ? this.problemLevel() : null,
                status: this.problemStatus() !== null ? this.problemStatus() : null,
                minCreatedAt: this.problemDateFrom() || null,
                maxCreatedAt: this.problemDateTo() || null
            };
        }

        this.filterChange.emit(payload);
    }

    reset(): void {
        this.tagsInput.set('');
        this.topicsInput.set('');
        this.postDateFrom.set('');
        this.postDateTo.set('');
        this.profileSkillsInput.set('');
        this.projectStatus.set(null);
        this.projectSkillsInput.set('');
        this.projectMinContributors.set(null);
        this.projectMaxContributors.set(null);
        this.projectMinRate.set(null);
        this.projectDateFrom.set('');
        this.projectDateTo.set('');
        this.communityType.set(null);
        this.jobType.set(null);
        this.jobSalaryType.set(null);
        this.jobMinSalary.set(null);
        this.jobMaxSalary.set(null);
        this.jobDateFrom.set('');
        this.jobDateTo.set('');
        this.problemLevel.set(null);
        this.problemStatus.set(null);
        this.problemDateFrom.set('');
        this.problemDateTo.set('');

        this.resetFilters.emit();
    }

    private parseArray(val: string): string[] | null {
        if (!val || !val.trim()) return null;
        return val.split(',').map(s => s.trim()).filter(s => s.length > 0);
    }
}
