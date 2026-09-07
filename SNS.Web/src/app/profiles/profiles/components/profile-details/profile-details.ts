import { Component, OnInit, inject, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideMapPin,
    LucideCalendar,
    LucideGlobe,
    LucideUserPlus,
    LucideUserCheck,
    LucideShare2,
    LucideMoreVertical,
    LucideUser,
    LucideCode,
    LucideGraduationCap,
    LucideFileText,
    LucideTerminal,
    LucideLayers,
    LucideCopy,
    LucideExternalLink,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideSettings
} from '@lucide/angular';
import { ProfilesService } from '../../services/profiles.service';
import { PostsService } from '../../../../content-management/posts/services/posts.service';
import { ProblemsService } from '../../../../discussions/problems/problems/services/problems.service';
import { ResumesService } from '../../../../resumes/resumes/services/resumes.service';
import { FollowsService } from '../../../social-graph/services/follows.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { ToastService } from '../../../../identity/notifications/services/toast.service';
import { ProfileDetailsDto } from '../../contracts/profile-details.dto';
import { PostOverviewDto } from '../../../../content-management/posts/contracts/post-model.dto';
import { ProblemSummaryDto } from '../../../../discussions/problems/problems/contracts/problem-summary.dto';
import { ResumeSummaryDto } from '../../../../resumes/resumes/contracts/resume-summary.dto';
import { Post } from '../../../../content-management/posts/components/post/post';
import { Problem } from '../../../../discussions/problems/problems/components/problem/problem';
import { AppPagination } from '../../../../shared/design-system/components/app-pagination/app-pagination';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { CircleLoader } from '../../../../shared/Loading/components/circle-loader/circle-loader';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';

import { AppAvatar } from '../../../../shared/design-system/components/app-avatar/app-avatar';

export type ProfileTab = 'posts' | 'problems' | 'resumes';

@Component({
    selector: 'app-profile-details',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        AppAvatar,
        Post,
        Problem,
        AppPagination,
        SkeletonLoaderComponent,
        CircleLoader,
        LucideArrowLeft,
        LucideMapPin,
        LucideCalendar,
        LucideGlobe,
        LucideUserPlus,
        LucideUserCheck,
        LucideShare2,
        LucideMoreVertical,
        LucideUser,
        LucideCode,
        LucideGraduationCap,
        LucideFileText,
        LucideTerminal,
        LucideLayers,
        LucideCopy,
        LucideExternalLink,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideSettings
    ],
    templateUrl: './profile-details.html',
    styleUrl: './profile-details.css'
})
export class ProfileDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private profilesService = inject(ProfilesService);
    private postsService = inject(PostsService);
    private problemsService = inject(ProblemsService);
    private resumesService = inject(ResumesService);
    private followsService = inject(FollowsService);
    private authService = inject(AuthenticationService);
    private toastService = inject(ToastService);
    private translate = inject(TranslateService);
    private languageService = inject(LanguageService);

    readonly SkeletonType = SkeletonType;

    // Route state
    profileId = signal<string>('');

    // Profile Details state
    profile = signal<ProfileDetailsDto | null>(null);
    isLoadingProfile = signal<boolean>(true);
    hasProfileError = signal<boolean>(false);
    isFollowing = signal<boolean>(false);
    isFollowLoading = signal<boolean>(false);

    currentProfileId = computed(() => this.authService.getProfileId() || this.authService.getUserId());

    isOwner = computed(() => {
        const myId = this.currentProfileId();
        const prof = this.profile();
        if (prof?.isViewerOwner) return true;
        return !!(myId && this.profileId() && myId.toLowerCase() === this.profileId().toLowerCase());
    });

    // Active Tab state ('posts' is default)
    activeTab = signal<ProfileTab>('posts');

    // Posts tab state
    posts = signal<PostOverviewDto[]>([]);
    isLoadingPosts = signal<boolean>(false);
    hasPostsError = signal<boolean>(false);
    postsCurrentPage = signal<number>(1);
    postsTotalPages = signal<number>(1);
    readonly postsPageSize = 10;

    // Problems tab state
    problems = signal<ProblemSummaryDto[]>([]);
    isLoadingProblems = signal<boolean>(false);
    hasProblemsError = signal<boolean>(false);
    problemsCurrentPage = signal<number>(1);
    problemsTotalPages = signal<number>(1);
    readonly problemsPageSize = 10;

    // Resumes tab state
    resumes = signal<ResumeSummaryDto[]>([]);
    isLoadingResumes = signal<boolean>(false);
    hasResumesError = signal<boolean>(false);

    isRtl = computed(() => {
        return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
            (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
    });

    initials = computed(() => {
        const name = this.profile()?.fullName?.trim();
        if (!name) return '??';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('profileId') || params.get('id');
            if (id && id !== this.profileId()) {
                this.profileId.set(id);
                this.loadProfile();
            }
        });
    }

    loadProfile(): void {
        const id = this.profileId();
        if (!id) return;

        this.isLoadingProfile.set(true);
        this.hasProfileError.set(false);

        this.profilesService.getProfileById(id).subscribe({
            next: res => {
                this.isLoadingProfile.set(false);
                if (res?.isSuccess && res.value) {
                    this.profile.set(res.value);
                    this.isFollowing.set(res.value.isFollowedByViewer);

                    // Load default tab (Posts)
                    this.loadTabContent(this.activeTab());
                } else {
                    this.hasProfileError.set(true);
                }
            },
            error: () => {
                this.isLoadingProfile.set(false);
                this.hasProfileError.set(true);
            }
        });
    }

    onSelectTab(tab: ProfileTab): void {
        if (this.activeTab() === tab) return;
        this.activeTab.set(tab);
        this.loadTabContent(tab);
    }

    loadTabContent(tab: ProfileTab): void {
        if (tab === 'posts') {
            this.loadPosts();
        } else if (tab === 'problems') {
            this.loadProblems();
        } else if (tab === 'resumes') {
            this.loadResumes();
        }
    }

    loadPosts(page: number = 1): void {
        const id = this.profileId();
        if (!id) return;

        this.isLoadingPosts.set(true);
        this.hasPostsError.set(false);
        this.postsCurrentPage.set(page);

        this.postsService.getUserPosts(id, page, this.postsPageSize).subscribe({
            next: res => {
                this.isLoadingPosts.set(false);
                if (res?.isSuccess && res.value) {
                    this.posts.set(res.value.items || []);
                    const total = res.value.totalCount || 0;
                    this.postsTotalPages.set(Math.ceil(total / this.postsPageSize) || 1);
                } else {
                    this.posts.set([]);
                    this.postsTotalPages.set(1);
                }
            },
            error: () => {
                this.isLoadingPosts.set(false);
                this.hasPostsError.set(true);
            }
        });
    }

    loadProblems(page: number = 1): void {
        const id = this.profileId();
        if (!id) return;

        this.isLoadingProblems.set(true);
        this.hasProblemsError.set(false);
        this.problemsCurrentPage.set(page);

        this.problemsService.getProblemsByAuthor(id, this.problemsPageSize, page).subscribe({
            next: res => {
                this.isLoadingProblems.set(false);
                if (res?.isSuccess && res.value) {
                    this.problems.set(res.value.items || []);
                    const total = res.value.totalCount || 0;
                    this.problemsTotalPages.set(Math.ceil(total / this.problemsPageSize) || 1);
                } else {
                    this.problems.set([]);
                    this.problemsTotalPages.set(1);
                }
            },
            error: () => {
                this.isLoadingProblems.set(false);
                this.hasProblemsError.set(true);
            }
        });
    }

    loadResumes(): void {
        const isOwner = this.profile()?.isViewerOwner;
        if (!isOwner) {
            // Non-owners don't have access to private resumes endpoint
            this.resumes.set([]);
            return;
        }

        this.isLoadingResumes.set(true);
        this.hasResumesError.set(false);

        this.resumesService.getMyResumes().subscribe({
            next: res => {
                this.isLoadingResumes.set(false);
                if (res?.isSuccess && res.value) {
                    this.resumes.set(res.value || []);
                } else {
                    this.resumes.set([]);
                }
            },
            error: () => {
                this.isLoadingResumes.set(false);
                this.hasResumesError.set(true);
            }
        });
    }

    onPostsPageChange(page: number): void {
        this.loadPosts(page);
    }

    onProblemsPageChange(page: number): void {
        this.loadProblems(page);
    }

    toggleFollow(): void {
        const id = this.profileId();
        if (!id || this.isFollowLoading()) return;

        const willFollow = !this.isFollowing();
        this.isFollowing.set(willFollow);
        this.isFollowLoading.set(true);

        const request$ = willFollow
            ? this.followsService.followProfile(id)
            : this.followsService.unfollowProfile(id);

        request$.subscribe({
            next: res => {
                this.isFollowLoading.set(false);
                if (!res?.isSuccess) {
                    this.isFollowing.set(!willFollow);
                    this.toastService.error(
                        this.translate.instant('Common.Error') || 'Error',
                        this.translate.instant('Profile.Follow_Error') || 'Failed to update follow status.'
                    );
                } else {
                    this.profile.update(p => p ? {
                        ...p,
                        followersCount: p.followersCount + (willFollow ? 1 : -1)
                    } : null);
                }
            },
            error: () => {
                this.isFollowLoading.set(false);
                this.isFollowing.set(!willFollow);
                this.toastService.error(
                    this.translate.instant('Common.Error') || 'Error',
                    'An error occurred while updating follow status.'
                );
            }
        });
    }

    onPostCommentsClick(postId: string): void {
        this.router.navigate(['../../post', postId], { relativeTo: this.route });
    }

    onProblemClick(problemId: string): void {
        this.router.navigate(['../../problem', problemId], { relativeTo: this.route });
    }

    navigateToSettings(): void {
        this.router.navigate(['/home/profiles', this.profileId(), 'settings']);
    }

    navigateToFollowers(): void {
        this.router.navigate(['followers'], { relativeTo: this.route });
    }

    navigateToFollowing(): void {
        this.router.navigate(['following'], { relativeTo: this.route });
    }

    copyProfileLink(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    goBack(): void {
        if (this.router.url.includes('/search/')) {
            this.router.navigate(['/home/search']);
        } else if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home']);
        }
    }
}
