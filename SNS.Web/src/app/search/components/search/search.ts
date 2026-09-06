import { Component, signal, inject, computed, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute, NavigationEnd, RouterOutlet } from '@angular/router';
import { map, filter } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideSearch,
    LucideSlidersHorizontal,
    LucideHistory,
    LucideTrendingUp,
    LucideLightbulb,
    LucideX,
    LucideRefreshCw,
    LucideAlertCircle
} from '@lucide/angular';
import { SearchService } from '../../service/search.service';
import {
    SearchCategory,
    GlobalSearchResultDto,
    SearchResult
} from '../../contracts/search.dto';
import { ProfileSummaryDto } from '../../../profiles/profiles/contracts/profile-summary.dto';
import { ProjectOverviewDto } from '../../../projects/contracts/project-summary.dto';
import { CommunitySummaryDto } from '../../../content-management/communities/communities/contracts/community-summary.dto';
import { JobSummaryDto } from '../../../jobs/jobs/contracts/job-summary.dto';
import { ProblemSummaryDto } from '../../../discussions/problems/problems/contracts/problem-summary.dto';
import { PostOverviewDto } from '../../../content-management/posts/contracts/post-model.dto';
import { Profile } from '../../../profiles/profiles/components/profile/profile';
import { Community } from '../../../content-management/communities/communities/components/community/community';
import { Job } from '../../../jobs/jobs/components/job/job';
import { Problem } from '../../../discussions/problems/problems/components/problem/problem';
import { SearchFilters } from '../search-filters/search-filters';
import { Post } from '../../../content-management/posts/components/post/post';
import { Project } from '../../../projects/components/project/project';
import { AppPagination } from '../../../shared/design-system/components/app-pagination/app-pagination';
import { SkeletonLoaderComponent, SkeletonType } from '../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { OverlayModule, ConnectionPositionPair } from '@angular/cdk/overlay';
import { LanguageService } from '../../../shared/services/language.service';
import { SupportedLanguage } from '../../../shared/contracts/supported-language.enum';

@Component({
    selector: 'app-search',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        TranslatePipe,
        RouterOutlet,
        OverlayModule,
        LucideSearch,
        LucideSlidersHorizontal,
        LucideHistory,
        LucideTrendingUp,
        LucideLightbulb,
        LucideX,
        LucideRefreshCw,
        LucideAlertCircle,
        Profile,
        Community,
        Job,
        Problem,
        SearchFilters,
        Post,
        Project,
        AppPagination,
        SkeletonLoaderComponent
    ],
    templateUrl: './search.html',
    styleUrl: './search.css'
})
export class Search implements OnInit {
    private searchService = inject(SearchService);
    private breakpointObserver = inject(BreakpointObserver);
    private languageService = inject(LanguageService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    // Route Detection for Child Details
    readonly isSearchRootRoute = toSignal(
        this.router.events.pipe(
            filter((event): event is NavigationEnd => event instanceof NavigationEnd),
            map(event => this.checkIsSearchRoot(event.urlAfterRedirects))
        ),
        {
            initialValue: this.checkIsSearchRoot(this.router.url)
        }
    );

    private savedScrollPosition = 0;

    constructor() {
        // Scroll restoration when returning to root search
        effect(() => {
            if (this.isSearchRootRoute() && this.savedScrollPosition > 0) {
                const scrollY = this.savedScrollPosition;
                setTimeout(() => {
                    window.scrollTo({ top: scrollY, behavior: 'instant' });
                }, 0);
            }
        });
    }

    private checkIsSearchRoot(rawUrl: string): boolean {
        const url = rawUrl.split('?')[0].split('#')[0];
        return url === '/search' || url === '/home/search';
    }

    isRtl = computed(() => {
        return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
            (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
    });

    filterPositions = computed<ConnectionPositionPair[]>(() => {
        const rtl = this.isRtl();
        return [
            new ConnectionPositionPair(
                { originX: rtl ? 'start' : 'end', originY: 'bottom' },
                { overlayX: rtl ? 'start' : 'end', overlayY: 'top' },
                0,
                8
            ),
            new ConnectionPositionPair(
                { originX: rtl ? 'end' : 'start', originY: 'bottom' },
                { overlayX: rtl ? 'end' : 'start', overlayY: 'top' },
                0,
                8
            )
        ];
    });

    readonly SkeletonType = SkeletonType;
    readonly categories: SearchCategory[] = [
        'People',
        'Posts',
        'Projects',
        'Communities',
        'Jobs',
        'Problems'
    ];

    // Mobile detection for bottom-sheet filter display
    isMobile = toSignal(
        this.breakpointObserver.observe('(max-width: 768px)').pipe(
            map(result => result.matches)
        ),
        { initialValue: false }
    );

    // Search state
    searchInput = signal<string>('');
    activeCategory = signal<SearchCategory | null>(null);
    isFilterOpen = signal<boolean>(false);
    activeFilters = signal<any>({});

    // Pagination state
    currentPage = signal<number>(1);
    pageSize = signal<number>(10);
    totalItems = signal<number>(0);

    totalPages = computed(() => {
        const total = this.totalItems();
        const size = this.pageSize();
        return total > 0 ? Math.ceil(total / size) : 0;
    });

    // Loading & Error states
    isLoading = signal<boolean>(false);
    hasError = signal<boolean>(false);
    hasSearched = signal<boolean>(false);

    // Results state
    globalResults = signal<GlobalSearchResultDto | null>(null);
    postsResults = signal<PostOverviewDto[]>([]);
    profilesResults = signal<ProfileSummaryDto[]>([]);
    projectsResults = signal<ProjectOverviewDto[]>([]);
    communitiesResults = signal<CommunitySummaryDto[]>([]);
    jobsResults = signal<JobSummaryDto[]>([]);
    problemsResults = signal<ProblemSummaryDto[]>([]);

    // Discovery static presets matching Image 1
    recentSearches = [
        'Machine Learning Ethics',
        'Stanford Alumni 2023',
        'Web3 Research Grant'
    ];

    trendingTopics = [
        'Large Language Models',
        'Fusion Energy Breakthrough',
        'Bioinformatics Tooling'
    ];

    suggestedForYou = [
        'Physics Communities',
        'Open Source Papers',
        'Local Meetups'
    ];

    ngOnInit(): void {
        // Initial state: do NOT execute search automatically.
    }

    onSearchSubmit(): void {
        if (!this.activeCategory()) {
            this.activeCategory.set('People');
        }
        this.hasSearched.set(true);
        this.currentPage.set(1);
        this.loadResults();
    }

    onSelectCategory(category: SearchCategory): void {
        if (this.activeCategory() === category) return;
        this.activeCategory.set(category);
        this.currentPage.set(1);
        this.activeFilters.set({});
        if (this.hasSearched()) {
            this.loadResults();
        }
    }

    toggleFilter(): void {
        this.isFilterOpen.update(v => !v);
    }

    closeFilter(): void {
        this.isFilterOpen.set(false);
    }

    onFilterChange(filters: any): void {
        this.activeFilters.set(filters);
        this.currentPage.set(1);
        this.closeFilter();
        this.hasSearched.set(true);
        this.loadResults();
    }

    onResetFilters(): void {
        this.activeFilters.set({});
        this.currentPage.set(1);
        this.closeFilter();
        if (this.hasSearched()) {
            this.loadResults();
        }
    }

    onPageChange(page: number): void {
        this.currentPage.set(page);
        this.loadResults();
        if (typeof window !== 'undefined') {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    onPickTopic(topic: string): void {
        this.searchInput.set(topic);
        if (!this.activeCategory()) {
            this.activeCategory.set('People');
        }
        this.hasSearched.set(true);
        this.currentPage.set(1);
        this.loadResults();
    }

    loadResults(): void {
        const term = this.searchInput().trim();
        const category = this.activeCategory();
        const page = this.currentPage();
        const size = this.pageSize();
        const filters = this.activeFilters();

        if (!category) {
            return;
        }

        this.isLoading.set(true);
        this.hasError.set(false);

        if (category === 'Posts') {
            this.searchService.searchPosts({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.postsResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.postsResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        } else if (category === 'People') {
            this.searchService.searchProfiles({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.profilesResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.profilesResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        } else if (category === 'Projects') {
            this.searchService.searchProjects({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.projectsResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.projectsResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        } else if (category === 'Communities') {
            this.searchService.searchCommunities({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.communitiesResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.communitiesResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        } else if (category === 'Jobs') {
            this.searchService.searchJobs({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.jobsResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.jobsResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        } else if (category === 'Problems') {
            this.searchService.searchProblems({
                searchTerm: term,
                currentPage: page,
                page,
                pageSize: size,
                ...filters
            }).subscribe({
                next: res => {
                    this.isLoading.set(false);
                    if (res?.isSuccess && res.value) {
                        this.problemsResults.set(res.value.hits.map(h => h.document));
                        this.totalItems.set(res.value.total);
                    } else {
                        this.problemsResults.set([]);
                        this.totalItems.set(0);
                    }
                },
                error: () => {
                    this.isLoading.set(false);
                    this.hasError.set(true);
                }
            });
        }
    }

    retry(): void {
        this.loadResults();
    }

    hasAnyResults(): boolean {
        const cat = this.activeCategory();
        if (cat === 'Posts') return this.postsResults().length > 0;
        else if (cat === 'People') return this.profilesResults().length > 0;
        else if (cat === 'Projects') return this.projectsResults().length > 0;
        else if (cat === 'Communities') return this.communitiesResults().length > 0;
        else if (cat === 'Jobs') return this.jobsResults().length > 0;
        else if (cat === 'Problems') return this.problemsResults().length > 0;
        return false;
    }

    onProfileClicked(profileId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['profile', profileId], { relativeTo: this.route });
    }

    onPostCommentsClick(postId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['post', postId], { relativeTo: this.route });
    }

    onProjectClicked(projectId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['project', projectId], { relativeTo: this.route });
    }

    onCommunityClicked(communityId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['community', communityId], { relativeTo: this.route });
    }

    onJobClicked(jobId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['job', jobId], { relativeTo: this.route });
    }

    onProblemClicked(problemId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['problem', problemId], { relativeTo: this.route });
    }
}
