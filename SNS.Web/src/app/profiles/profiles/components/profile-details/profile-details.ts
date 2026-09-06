import { Component, OnInit, inject, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideMapPin,
    LucideCalendar,
    LucideGlobe,
    LucideUserPlus,
    LucideUserCheck,
    LucideShare2,
    LucideMoreVertical,
    LucideAward,
    LucideUser,
    LucideCode,
    LucideGraduationCap,
    LucideFileText,
    LucideTerminal,
    LucideLayers,
    LucideCopy,
    LucideExternalLink,
    LucideAlertCircle,
    LucideRefreshCw
} from '@lucide/angular';
import { ProfilesService } from '../../services/profiles.service';
import { PostsService } from '../../../../content-management/posts/services/posts.service';
import { ProblemsService } from '../../../../discussions/problems/problems/services/problems.service';
import { ResumesService } from '../../../../resumes/resumes/services/resumes.service';
import { ProfileDetailsDto } from '../../contracts/profile-details.dto';
import { PostOverviewDto } from '../../../../content-management/posts/contracts/post-model.dto';
import { ProblemSummaryDto } from '../../../../discussions/problems/problems/contracts/problem-summary.dto';
import { ResumeSummaryDto } from '../../../../resumes/resumes/contracts/resume-summary.dto';
import { Post } from '../../../../content-management/posts/components/post/post';
import { Problem } from '../../../../discussions/problems/problems/components/problem/problem';
import { AppPagination } from '../../../../shared/design-system/components/app-pagination/app-pagination';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';

export type ProfileTab = 'posts' | 'problems' | 'resumes';

@Component({
    selector: 'app-profile-details',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        Post,
        Problem,
        AppPagination,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideMapPin,
        LucideCalendar,
        LucideGlobe,
        LucideUserPlus,
        LucideUserCheck,
        LucideShare2,
        LucideMoreVertical,
        LucideAward,
        LucideUser,
        LucideCode,
        LucideGraduationCap,
        LucideFileText,
        LucideTerminal,
        LucideLayers,
        LucideCopy,
        LucideExternalLink,
        LucideAlertCircle,
        LucideRefreshCw
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
    private languageService = inject(LanguageService);

    readonly SkeletonType = SkeletonType;
    readonly defaultAvatar = 'assets/images/default-avatar.png';

    // Route state
    profileId = signal<string>('');

    // Profile Details state
    profile = signal<ProfileDetailsDto | null>(null);
    isLoadingProfile = signal<boolean>(true);
    hasProfileError = signal<boolean>(false);
    isFollowing = signal<boolean>(false);

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
        this.isFollowing.update(f => !f);
    }

    onAvatarError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultAvatar) {
            target.src = this.defaultAvatar;
        }
    }

    onPostCommentsClick(postId: string): void {
        this.router.navigate(['../../post', postId], { relativeTo: this.route });
    }

    onProblemClick(problemId: string): void {
        this.router.navigate(['../../problem', problemId], { relativeTo: this.route });
    }

    copyProfileLink(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
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
