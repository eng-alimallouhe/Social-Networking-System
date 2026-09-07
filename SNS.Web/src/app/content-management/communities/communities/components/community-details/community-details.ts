import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideGlobe,
    LucideLock,
    LucideCalendar,
    LucideUsers,
    LucideMessageSquare,
    LucideCheck,
    LucideUserPlus,
    LucideShare2,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideScrollText,
    LucideShield,
    LucideSend,
    LucideCheckCircle2,
    LucideXCircle,
    LucideClock,
    LucideSettings,
    LucideHelpCircle,
    LucideX
} from '@lucide/angular';
import { CommunitiesService } from '../../services/communities.service';
import { CommunityMembershipsService } from '../../../memberships/services/community-memberships.service';
import { PostsService } from '../../../../posts/services/posts.service';
import { ProblemsService } from '../../../../../discussions/problems/problems/services/problems.service';
import { ToastService } from '../../../../../identity/notifications/services/toast.service';
import { CommunityDetailsDto } from '../../contracts/community-details.dto';
import { CommunityMemberDto } from '../../../memberships/contracts/community-member.dto';
import { MembershipRequestDto } from '../../../memberships/contracts/membership-request.dto';
import { PostOverviewDto } from '../../../../posts/contracts/post-model.dto';
import { ProblemSummaryDto } from '../../../../../discussions/problems/problems/contracts/problem-summary.dto';
import { CommunityType } from '../../../../../shared/contracts/community-type';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { Post } from '../../../../posts/components/post/post';
import { Problem } from '../../../../../discussions/problems/problems/components/problem/problem';
import { AppAvatar } from '../../../../../shared/design-system/components/app-avatar/app-avatar';
import { getInitials } from '../../../../../shared/utils/avatar-utils';

type CommunityTab = 'about' | 'posts' | 'problems' | 'members' | 'management';

@Component({
    selector: 'app-community-details',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        TranslatePipe,
        SkeletonLoaderComponent,
        Post,
        Problem,
        AppAvatar,
        LucideArrowLeft,
        LucideGlobe,
        LucideLock,
        LucideCalendar,
        LucideUsers,
        LucideMessageSquare,
        LucideCheck,
        LucideUserPlus,
        LucideShare2,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideScrollText,
        LucideShield,
        LucideSend,
        LucideCheckCircle2,
        LucideXCircle,
        LucideClock,
        LucideSettings,
        LucideHelpCircle,
        LucideX
    ],
    templateUrl: './community-details.html',
    styleUrl: './community-details.css'
})
export class CommunityDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private communitiesService = inject(CommunitiesService);
    private membershipsService = inject(CommunityMembershipsService);
    private postsService = inject(PostsService);
    private problemsService = inject(ProblemsService);
    private toastService = inject(ToastService);
    private translate = inject(TranslateService);

    readonly SkeletonType = SkeletonType;
    readonly CommunityType = CommunityType;

    communityId = signal<string>('');
    community = signal<CommunityDetailsDto | null>(null);
    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);

    // Active Tab
    activeTab = signal<CommunityTab>('about');

    // Membership & Moderation State
    isMember = signal<boolean>(false);
    isManager = signal<boolean>(false);
    hasPendingJoinRequest = signal<boolean>(false);
    hasPendingPost = signal<boolean>(false);

    // Action loaders
    isJoining = signal<boolean>(false);
    isCancellingRequest = signal<boolean>(false);

    // Posts tab state
    posts = signal<PostOverviewDto[]>([]);
    isLoadingPosts = signal<boolean>(false);
    postsPage = signal<number>(1);
    hasMorePosts = signal<boolean>(false);

    // Create post form state
    newPostTitle = signal<string>('');
    newPostContent = signal<string>('');
    isSubmittingPost = signal<boolean>(false);

    // Problems tab state
    problems = signal<ProblemSummaryDto[]>([]);
    isLoadingProblems = signal<boolean>(false);
    problemsPage = signal<number>(1);
    hasMoreProblems = signal<boolean>(false);

    // Members tab state
    members = signal<CommunityMemberDto[]>([]);
    isLoadingMembers = signal<boolean>(false);
    membersPage = signal<number>(1);
    hasMoreMembers = signal<boolean>(false);

    // Management tab state
    mgmtActiveSubTab = signal<'pending-posts' | 'join-requests'>('pending-posts');
    pendingPosts = signal<PostOverviewDto[]>([]);
    isLoadingPendingPosts = signal<boolean>(false);
    joinRequests = signal<MembershipRequestDto[]>([]);
    isLoadingJoinRequests = signal<boolean>(false);

    initials = computed(() => {
        return getInitials(this.community()?.name);
    });

    isPublic = computed(() => {
        return this.community()?.type === CommunityType.Public;
    });

    canAccessContent = computed(() => {
        const c = this.community();
        if (!c) return false;
        return c.isMember || c.type === CommunityType.Public;
    });

    readonly formattedDescription = computed<string>(() => {
        const desc = this.community()?.description;
        if (!desc) return '';
        return desc.replace(/\\n/g, '\n');
    });

    readonly formattedRules = computed<string[]>(() => {
        const rules = this.community()?.rulesText;
        if (!rules) return [];
        const unescaped = rules.replace(/\\n/g, '\n').replace(/\\r/g, '');
        return unescaped
            .split('\n')
            .map(r => r.trim())
            .filter(r => r.length > 0);
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('communityId') || params.get('id');
            if (id && id !== this.communityId()) {
                this.communityId.set(id);
                this.loadCommunity();
            }
        });
    }

    loadCommunity(): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoading.set(true);
        this.hasError.set(false);

        this.communitiesService.getCommunityById(id).subscribe({
            next: res => {
                this.isLoading.set(false);
                if (res?.isSuccess && res.value) {
                    const data = res.value;
                    this.community.set(data);
                    this.isMember.set(data.isMember);
                    this.isManager.set(data.isManager);
                    this.hasPendingJoinRequest.set(data.hasPendingJoinRequest);
                    this.hasPendingPost.set(data.hasPendingPost);

                    // If private and not member, force activeTab to about
                    if (!this.canAccessContent() && this.activeTab() !== 'about') {
                        this.activeTab.set('about');
                    }
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

    setTab(tab: CommunityTab): void {
        if (!this.canAccessContent() && tab !== 'about') {
            return;
        }
        if (tab === 'management' && !this.isManager()) {
            return;
        }

        this.activeTab.set(tab);

        if (tab === 'posts' && this.posts().length === 0) {
            this.loadPosts();
        } else if (tab === 'problems' && this.problems().length === 0) {
            this.loadProblems();
        } else if (tab === 'members' && this.members().length === 0) {
            this.loadMembers();
        } else if (tab === 'management') {
            this.loadManagementData();
        }
    }

    // Membership Actions
    joinCommunity(): void {
        const id = this.communityId();
        if (!id || this.isJoining()) return;

        this.isJoining.set(true);
        this.membershipsService.joinCommunity(id).subscribe({
            next: res => {
                this.isJoining.set(false);
                if (res?.isSuccess) {
                    if (this.isPublic()) {
                        this.isMember.set(true);
                        this.toastService.success(this.translate.instant('Community.View_Community'), this.translate.instant('Community.Toasts.Joined_Success'));
                        if (this.community()) {
                            this.community.update(c => c ? { ...c, isMember: true, membersCount: c.membersCount + 1 } : null);
                        }
                    } else {
                        this.hasPendingJoinRequest.set(true);
                        this.toastService.info(this.translate.instant('Community.View_Community'), this.translate.instant('Community.Toasts.Join_Pending'));
                        if (this.community()) {
                            this.community.update(c => c ? { ...c, hasPendingJoinRequest: true } : null);
                        }
                    }
                } else {
                    this.toastService.error(this.translate.instant('Community.View_Community'), this.translate.instant('Community.Toasts.Join_Failed'));
                }
            },
            error: () => {
                this.isJoining.set(false);
                this.toastService.error(this.translate.instant('Community.View_Community'), this.translate.instant('Community.Toasts.Join_Failed'));
            }
        });
    }

    cancelJoinRequest(): void {
        const id = this.communityId();
        if (!id || this.isCancellingRequest()) return;

        this.isCancellingRequest.set(true);
        this.membershipsService.cancelJoinRequest(id).subscribe({
            next: (res: any) => {
                this.isCancellingRequest.set(false);
                if (res?.isSuccess) {
                    this.hasPendingJoinRequest.set(false);
                    this.toastService.info(this.translate.instant('Community.Cancel_Request'), this.translate.instant('Community.Toasts.Cancel_Success'));
                    if (this.community()) {
                        this.community.update(c => c ? { ...c, hasPendingJoinRequest: false } : null);
                    }
                } else {
                    this.toastService.error(this.translate.instant('Community.Cancel_Request'), this.translate.instant('Community.Toasts.Cancel_Failed'));
                }
            },
            error: () => {
                this.isCancellingRequest.set(false);
                this.toastService.error(this.translate.instant('Community.Cancel_Request'), this.translate.instant('Community.Toasts.Cancel_Failed'));
            }
        });
    }

    share(): void {
        if (typeof window !== 'undefined') {
            const url = window.location.href;
            if (navigator.share) {
                navigator.share({
                    title: this.community()?.name || 'Community',
                    url: url
                }).catch(() => {
                    // Handled or cancelled
                });
            } else if (navigator.clipboard) {
                navigator.clipboard.writeText(url).then(() => {
                    this.toastService.success(this.translate.instant('Community.Share'), this.translate.instant('Community.Toasts.Link_Copied'));
                }).catch(() => {
                    this.toastService.success(this.translate.instant('Community.Share'), this.translate.instant('Community.Toasts.Link_Copied'));
                });
            }
        }
    }

    // Posts tab actions
    loadPosts(page: number = 1): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoadingPosts.set(true);
        this.postsService.getCommunityPosts(id, page, 10).subscribe({
            next: res => {
                this.isLoadingPosts.set(false);
                if (res?.isSuccess && res.value) {
                    const paged = res.value;
                    if (page === 1) {
                        this.posts.set(paged.items);
                    } else {
                        this.posts.update(current => [...current, ...paged.items]);
                    }
                    this.postsPage.set(page);
                    this.hasMorePosts.set(paged.items.length === 10);
                }
            },
            error: () => {
                this.isLoadingPosts.set(false);
            }
        });
    }

    loadMorePosts(): void {
        if (!this.isLoadingPosts() && this.hasMorePosts()) {
            this.loadPosts(this.postsPage() + 1);
        }
    }

    submitNewPost(): void {
        const id = this.communityId();
        const title = this.newPostTitle().trim();
        const content = this.newPostContent().trim();

        if (!id || !title || !content || this.isSubmittingPost()) return;

        this.isSubmittingPost.set(true);
        this.postsService.createPost({
            communityId: id,
            title: title,
            content: content,
            isPenned: false,
            files: []
        }).subscribe({
            next: (res: any) => {
                this.isSubmittingPost.set(false);
                if (res?.isSuccess) {
                    this.newPostTitle.set('');
                    this.newPostContent.set('');
                    this.toastService.success(this.translate.instant('Community.Create_Post'), this.translate.instant('Community.Toasts.Post_Submitted'));

                    // If post was sent for classification/pending approval
                    this.loadCommunity();
                    this.loadPosts(1);
                } else {
                    this.toastService.error(this.translate.instant('Community.Create_Post'), this.translate.instant('Community.Toasts.Post_Failed'));
                }
            },
            error: () => {
                this.isSubmittingPost.set(false);
                this.toastService.error(this.translate.instant('Community.Create_Post'), this.translate.instant('Community.Toasts.Post_Failed'));
            }
        });
    }

    // Problems tab actions
    loadProblems(page: number = 1): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoadingProblems.set(true);
        this.problemsService.getProblemsByCommunity(id, 10, page).subscribe({
            next: res => {
                this.isLoadingProblems.set(false);
                if (res?.isSuccess && res.value) {
                    const paged = res.value;
                    if (page === 1) {
                        this.problems.set(paged.items);
                    } else {
                        this.problems.update(current => [...current, ...paged.items]);
                    }
                    this.problemsPage.set(page);
                    this.hasMoreProblems.set(paged.items.length === 10);
                }
            },
            error: () => {
                this.isLoadingProblems.set(false);
            }
        });
    }

    loadMoreProblems(): void {
        if (!this.isLoadingProblems() && this.hasMoreProblems()) {
            this.loadProblems(this.problemsPage() + 1);
        }
    }

    // Members tab actions
    loadMembers(page: number = 1): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoadingMembers.set(true);
        this.membershipsService.getCommunityMembers(id, null, page, 20).subscribe({
            next: res => {
                this.isLoadingMembers.set(false);
                if (res?.isSuccess && res.value) {
                    const paged = res.value;
                    if (page === 1) {
                        this.members.set(paged.items);
                    } else {
                        this.members.update(current => [...current, ...paged.items]);
                    }
                    this.membersPage.set(page);
                    this.hasMoreMembers.set(paged.items.length === 20);
                }
            },
            error: () => {
                this.isLoadingMembers.set(false);
            }
        });
    }

    loadMoreMembers(): void {
        if (!this.isLoadingMembers() && this.hasMoreMembers()) {
            this.loadMembers(this.membersPage() + 1);
        }
    }

    // Management tab actions
    loadManagementData(): void {
        if (this.mgmtActiveSubTab() === 'pending-posts') {
            this.loadPendingPosts();
        } else {
            this.loadJoinRequests();
        }
    }

    setMgmtSubTab(tab: 'pending-posts' | 'join-requests'): void {
        this.mgmtActiveSubTab.set(tab);
        this.loadManagementData();
    }

    loadPendingPosts(): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoadingPendingPosts.set(true);
        this.postsService.getPendingCommunityPosts(id, 1, 20).subscribe({
            next: res => {
                this.isLoadingPendingPosts.set(false);
                if (res?.isSuccess && res.value) {
                    this.pendingPosts.set(res.value.items);
                }
            },
            error: () => {
                this.isLoadingPendingPosts.set(false);
            }
        });
    }

    approvePost(postId: string): void {
        const id = this.communityId();
        if (!id) return;

        this.postsService.approveCommunityPost(id, postId).subscribe({
            next: (res: any) => {
                if (res?.isSuccess) {
                    this.pendingPosts.update(posts => posts.filter(p => p.id !== postId));
                    this.toastService.success(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Post_Approved'));
                    if (this.community()) {
                        this.community.update(c => c ? { ...c, postsCount: c.postsCount + 1 } : null);
                    }
                } else {
                    this.toastService.error(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Post_Approve_Failed'));
                }
            },
            error: () => {
                this.toastService.error(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Post_Approve_Failed'));
            }
        });
    }

    rejectPost(postId: string): void {
        const id = this.communityId();
        if (!id) return;

        this.postsService.rejectCommunityPost(id, postId).subscribe({
            next: (res: any) => {
                if (res?.isSuccess) {
                    this.pendingPosts.update(posts => posts.filter(p => p.id !== postId));
                    this.toastService.info(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Post_Rejected'));
                } else {
                    this.toastService.error(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Post_Reject_Failed'));
                }
            },
            error: () => {
                this.toastService.error(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Post_Reject_Failed'));
            }
        });
    }

    loadJoinRequests(): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoadingJoinRequests.set(true);
        this.membershipsService.getMembershipRequests(id, 1, 20).subscribe({
            next: (res: any) => {
                this.isLoadingJoinRequests.set(false);
                if (res?.isSuccess && res.value) {
                    this.joinRequests.set(res.value.items);
                }
            },
            error: () => {
                this.isLoadingJoinRequests.set(false);
            }
        });
    }

    approveRequest(requestId: string): void {
        const id = this.communityId();
        if (!id) return;

        this.membershipsService.approveMembershipRequest(id, requestId).subscribe({
            next: (res: any) => {
                if (res?.isSuccess) {
                    this.joinRequests.update(reqs => reqs.filter(r => r.requestId !== requestId));
                    this.toastService.success(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Request_Approved'));
                    if (this.community()) {
                        this.community.update(c => c ? { ...c, membersCount: c.membersCount + 1 } : null);
                    }
                } else {
                    this.toastService.error(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Request_Approve_Failed'));
                }
            },
            error: () => {
                this.toastService.error(this.translate.instant('Community.Approve'), this.translate.instant('Community.Toasts.Request_Approve_Failed'));
            }
        });
    }

    rejectRequest(requestId: string): void {
        const id = this.communityId();
        if (!id) return;

        this.membershipsService.rejectMembershipRequest(id, requestId).subscribe({
            next: (res: any) => {
                if (res?.isSuccess) {
                    this.joinRequests.update(reqs => reqs.filter(r => r.requestId !== requestId));
                    this.toastService.info(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Request_Rejected'));
                } else {
                    this.toastService.error(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Request_Reject_Failed'));
                }
            },
            error: () => {
                this.toastService.error(this.translate.instant('Community.Reject'), this.translate.instant('Community.Toasts.Request_Reject_Failed'));
            }
        });
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home/search']);
        }
    }
}
