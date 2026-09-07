import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
  LucideBuilding,
  LucideGlobe,
  LucideBriefcase,
  LucideUsers,
  LucideCalendar,
  LucideArrowLeft,
  LucideExternalLink,
  LucideShieldCheck
} from '@lucide/angular';
import { CompaniesService } from '../../services/companies.service';
import { JobsService } from '../../../jobs/services/jobs.service';
import { CompanyAdministratorsService } from '../../../company-administrators/services/company-administrators.service';
import { CompanyDetailsDto } from '../../contracts/company-details.dto';
import { JobSummaryDto } from '../../../jobs/contracts/job-summary.dto';
import { CompanyAdministratorDto } from '../../../company-administrators/contracts/company-administrator.dto';
import { CompanyRole } from '../../../enums/company-role.enum';
import { AppAvatar } from '../../../../shared/design-system/components/app-avatar/app-avatar';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { Job } from '../../../jobs/components/job/job';
import { ToastService } from '../../../../identity/notifications/services/toast.service';

export type CompanyTab = 'about' | 'jobs' | 'managers';

@Component({
  selector: 'app-company-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    LucideBuilding,
    LucideGlobe,
    LucideBriefcase,
    LucideUsers,
    LucideCalendar,
    LucideArrowLeft,
    LucideExternalLink,
    LucideShieldCheck,
    AppAvatar,
    SkeletonLoaderComponent,
    Job
  ],
  templateUrl: './company-details.html',
  styleUrl: './company-details.css'
})
export class CompanyDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private location = inject(Location);
  private companiesService = inject(CompaniesService);
  private jobsService = inject(JobsService);
  private administratorsService = inject(CompanyAdministratorsService);
  private toastService = inject(ToastService);

  readonly SkeletonType = SkeletonType;
  readonly CompanyRole = CompanyRole;

  companyId = signal<string>('');
  company = signal<CompanyDetailsDto | null>(null);
  isLoadingCompany = signal<boolean>(true);

  activeTab = signal<CompanyTab>('about');

  // Tab 2: Published Jobs (Separate Query & Lazy Loaded)
  jobs = signal<JobSummaryDto[]>([]);
  isLoadingJobs = signal<boolean>(false);
  hasLoadedJobs = signal<boolean>(false);

  // Tab 3: Company Managers (Separate Query & Lazy Loaded)
  managers = signal<CompanyAdministratorDto[]>([]);
  isLoadingManagers = signal<boolean>(false);
  hasLoadedManagers = signal<boolean>(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('companyId');
      if (id) {
        this.companyId.set(id);
        this.loadCompanyDetails(id);
      }
    });
  }

  loadCompanyDetails(id: string): void {
    this.isLoadingCompany.set(true);
    this.companiesService.getCompanyById(id).subscribe({
      next: (res) => {
        this.isLoadingCompany.set(false);
        if (res.isSuccess && res.value) {
          this.company.set(res.value);
        } else {
          this.toastService.error('Error', 'Company not found.');
        }
      },
      error: () => {
        this.isLoadingCompany.set(false);
        this.toastService.error('Error', 'Failed to load company details.');
      }
    });
  }

  setTab(tab: CompanyTab): void {
    this.activeTab.set(tab);

    if (tab === 'jobs' && !this.hasLoadedJobs()) {
      this.loadCompanyJobs();
    } else if (tab === 'managers' && !this.hasLoadedManagers()) {
      this.loadCompanyManagers();
    }
  }

  loadCompanyJobs(): void {
    const id = this.companyId();
    if (!id) return;

    this.isLoadingJobs.set(true);
    this.jobsService.getJobsByCompany(id, 20, 1).subscribe({
      next: (res) => {
        this.isLoadingJobs.set(false);
        this.hasLoadedJobs.set(true);
        if (res.isSuccess && res.value?.items) {
          this.jobs.set(res.value.items);
        }
      },
      error: () => {
        this.isLoadingJobs.set(false);
        this.hasLoadedJobs.set(true);
        this.toastService.error('Error', 'Failed to load company jobs.');
      }
    });
  }

  loadCompanyManagers(): void {
    const id = this.companyId();
    if (!id) return;

    this.isLoadingManagers.set(true);
    this.administratorsService.getCompanyAdministrators(id).subscribe({
      next: (res) => {
        this.isLoadingManagers.set(false);
        this.hasLoadedManagers.set(true);
        if (res.isSuccess && res.value) {
          this.managers.set(res.value);
        }
      },
      error: () => {
        this.isLoadingManagers.set(false);
        this.hasLoadedManagers.set(true);
        this.toastService.error('Error', 'Failed to load company managers.');
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
}
