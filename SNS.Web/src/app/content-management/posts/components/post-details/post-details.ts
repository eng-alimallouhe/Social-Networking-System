import { Component, OnInit, inject, signal, computed, viewChild, ElementRef, effect, DestroyRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
    LucideArrowLeft,
    LucideSend,
    LucideMessageSquare,
    LucideAlertCircle,
    LucideRefreshCw
} from '@lucide/angular';
import { PostsService } from '../../services/posts.service';
import { CommentsService } from '../../../comments/services/comments.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';
import { PostDetailsDto } from '../../contracts/post-details.dto';
import { PostModelDto } from '../../contracts/post-model.dto';
import { CommentSummaryDto } from '../../../comments/contracts/comment-summary.dto';
import { Post } from '../post/post';
import { Comment } from '../../../comments/components/comment/comment';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { WantToLogin } from '../../../../shared/components/want-to-login/want-to-login';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-post-details',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        Post,
        Comment,
        SkeletonLoaderComponent,
        WantToLogin,
        LucideArrowLeft,
        LucideSend,
        LucideMessageSquare,
        LucideAlertCircle,
        LucideRefreshCw,
        TranslatePipe
    ],
    templateUrl: './post-details.html',
    styleUrls: ['./post-details.css']
})
export class PostDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private postsService = inject(PostsService);
    private commentsService = inject(CommentsService);
    private authService = inject(AuthenticationService);
    private languageService = inject(LanguageService);
    private destroyRef = inject(DestroyRef);

    readonly SkeletonType = SkeletonType;

    // Post state
    postId = signal<string>('');
    postDetails = signal<PostDetailsDto | null>(null);
    isPostLoading = signal<boolean>(true);
    postError = signal<boolean>(false);

    // Comments state
    comments = signal<CommentSummaryDto[]>([]);
    totalCommentsCount = signal<number>(0);
    isCommentsLoading = signal<boolean>(false);
    isLoadingMoreComments = signal<boolean>(false);
    hasMoreComments = signal<boolean>(false);
    commentsPage = signal<number>(1);
    readonly pageSize = 10;
    commentsError = signal<boolean>(false);

    // Comment composer
    newCommentText = signal<string>('');
    isSubmittingComment = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);

    // Reply context (optional: replying to a specific author)
    replyingTo = signal<CommentSummaryDto | null>(null);

    // Sentinel for comments infinite scroll
    sentinel = viewChild<ElementRef<HTMLDivElement>>('commentsSentinel');
    private observer: IntersectionObserver | null = null;

    isRtl = computed(() => {
        return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
            (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
    });

    mappedPost = computed<PostModelDto | null>(() => {
        const details = this.postDetails();
        if (!details) return null;
        return {
            id: details.id,
            author: details.author,
            community: details.community,
            title: details.title,
            content: details.content,
            createdAt: details.createdAt,
            updatedAt: details.updatedAt,
            lastInteractedAt: null,
            media: details.media ?? [],
            tags: details.tags ?? [],
            commentsCount: this.totalCommentsCount(),
            reactionsCount: details.reactionCount ?? 0,
            viewsCount: details.engagementScore ?? 0,
            savesCount: details.saveCount ?? 0,
            currentUserReaction: null,
            mentions: details.mentions ?? []
        };
    });

    constructor() {
        effect(() => {
            const sentinelEl = this.sentinel()?.nativeElement;

            if (this.observer) {
                this.observer.disconnect();
                this.observer = null;
            }

            if (sentinelEl && typeof IntersectionObserver !== 'undefined') {
                this.observer = new IntersectionObserver(
                    entries => {
                        const entry = entries[0];
                        if (entry?.isIntersecting) {
                            this.loadMoreComments();
                        }
                    },
                    {
                        root: null,
                        rootMargin: '0px 0px 200px 0px',
                        threshold: 0.1
                    }
                );
                this.observer.observe(sentinelEl);
            }
        });

        this.destroyRef.onDestroy(() => {
            if (this.observer) {
                this.observer.disconnect();
                this.observer = null;
            }
        });
    }

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('postId');
            if (id && id !== this.postId()) {
                this.postId.set(id);
                this.loadPost();
            }
        });
    }

    loadPost(): void {
        const id = this.postId();
        if (!id) return;

        this.isPostLoading.set(true);
        this.postError.set(false);

        this.postsService.getPostById(id).subscribe({
            next: res => {
                this.isPostLoading.set(false);
                if (res.isSuccess && res.value) {
                    this.postDetails.set(res.value);

                    // Initialize comments from postDetails if present
                    if (res.value.comments?.items) {
                        this.comments.set(res.value.comments.items);
                        this.totalCommentsCount.set(res.value.comments.totalCount ?? res.value.comments.items.length);
                        this.commentsPage.set(res.value.comments.currentPage ?? 1);
                        this.hasMoreComments.set(
                            res.value.comments.hasNext ??
                            (res.value.comments.items.length < (res.value.comments.totalCount ?? 0))
                        );
                    } else {
                        this.loadInitialComments();
                    }
                } else {
                    this.postError.set(true);
                }
            },
            error: () => {
                this.isPostLoading.set(false);
                this.postError.set(true);
            }
        });
    }

    loadInitialComments(): void {
        const id = this.postId();
        if (!id) return;

        this.isCommentsLoading.set(true);
        this.commentsError.set(false);

        this.commentsService.getPostComments(id, 1, this.pageSize).subscribe({
            next: res => {
                this.isCommentsLoading.set(false);
                if (res.isSuccess && res.value) {
                    const items = res.value.items ?? [];
                    this.comments.set(items);
                    this.totalCommentsCount.set(res.value.totalCount ?? items.length);
                    this.commentsPage.set(1);
                    this.hasMoreComments.set(res.value.hasNext ?? (items.length < (res.value.totalCount ?? 0)));
                } else {
                    this.commentsError.set(true);
                }
            },
            error: () => {
                this.isCommentsLoading.set(false);
                this.commentsError.set(true);
            }
        });
    }

    loadMoreComments(): void {
        if (!this.hasMoreComments() || this.isLoadingMoreComments() || this.isCommentsLoading()) return;

        const id = this.postId();
        const nextPage = this.commentsPage() + 1;

        this.isLoadingMoreComments.set(true);

        this.commentsService.getPostComments(id, nextPage, this.pageSize).subscribe({
            next: res => {
                this.isLoadingMoreComments.set(false);
                if (res.isSuccess && res.value) {
                    const newItems = res.value.items ?? [];
                    this.comments.update(existing => [...existing, ...newItems]);
                    this.commentsPage.set(nextPage);
                    this.hasMoreComments.set(res.value.hasNext ?? (newItems.length >= this.pageSize));
                }
            },
            error: () => {
                this.isLoadingMoreComments.set(false);
            }
        });
    }

    onEnterKey(event: Event): void {
        const kbEvent = event as KeyboardEvent;
        if (kbEvent.ctrlKey || kbEvent.metaKey) {
            this.submitComment();
        }
    }

    onReplyToComment(comment: CommentSummaryDto): void {
        this.replyingTo.set(comment);
        if (comment.author?.fullName) {
            this.newCommentText.set(`@${comment.author.fullName} `);
        }
    }

    cancelReply(): void {
        this.replyingTo.set(null);
        this.newCommentText.set('');
    }

    submitComment(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }

        const content = this.newCommentText().trim();
        if (!content || this.isSubmittingComment()) return;

        this.isSubmittingComment.set(true);

        const parentId = this.replyingTo()?.id ?? null;

        this.commentsService.createComment({
            postId: this.postId(),
            parentCommentId: parentId,
            content
        }).subscribe({
            next: res => {
                this.isSubmittingComment.set(false);
                if (res.isSuccess) {
                    this.newCommentText.set('');
                    this.replyingTo.set(null);
                    // Refresh comments from page 1 to show the newly posted comment
                    this.loadInitialComments();
                }
            },
            error: () => {
                this.isSubmittingComment.set(false);
            }
        });
    }

    onCommentDeleted(commentId: string): void {
        this.comments.update(items => items.filter(c => c.id !== commentId));
        this.totalCommentsCount.update(c => Math.max(0, c - 1));
    }

    onPostDeleted(): void {
        this.goBack();
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home']);
        }
    }
}
