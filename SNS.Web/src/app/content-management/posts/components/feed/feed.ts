import { Component, signal, inject, effect, computed, viewChild, ElementRef, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute, NavigationEnd, RouterOutlet } from '@angular/router';
import { map, tap, filter } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideRefreshCw, LucideAlertCircle, LucideMessageSquare, LucideInfo } from '@lucide/angular';
import { PostsService } from '../../services/posts.service';
import { PostOverviewDto } from '../../contracts/post-model.dto';
import { Post } from '../post/post';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-feed',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        Post,
        SkeletonLoaderComponent,
        RouterOutlet,
        LucideRefreshCw,
        LucideAlertCircle,
        LucideMessageSquare,
        LucideInfo
    ],
    templateUrl: './feed.html',
    styleUrls: ['./feed.css']
})
export class Feed {
    private postsService = inject(PostsService);
    private destroyRef = inject(DestroyRef);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    readonly SkeletonType = SkeletonType;

    // Route State
    readonly isFeedRootRoute = toSignal(
        this.router.events.pipe(
            filter((event): event is NavigationEnd => event instanceof NavigationEnd),
            map(event => this.checkIsFeedRoot(event.urlAfterRedirects))
        ),
        {
            initialValue: this.checkIsFeedRoot(this.router.url)
        }
    );

    private savedScrollPosition = 0;

    // Pagination & Feed State
    currentPage = signal<number>(1);
    pageSize = signal<number>(10);
    hasNextPage = signal<boolean>(true);
    loadedPosts = signal<PostOverviewDto[]>([]);
    dismissedPostIds = signal<Set<string>>(new Set());

    // rxResource manages the HTTP request for the current page
    feedResource = rxResource({
        params: () => ({
            page: this.currentPage(),
            size: this.pageSize()
        }),
        stream: ({ params }) => {
            return this.postsService.getFeed(params.page, params.size).pipe(
                map(result => {
                    if (result && !result.isSuccess) {
                        throw result;
                    }
                    return result;
                }),
                tap(result => {
                    if (!result?.isSuccess || !result.value) return;
                    const newPosts = result.value;

                    if (newPosts.length === 0) {
                        this.hasNextPage.set(false);
                    } else {
                        this.hasNextPage.set(true);
                        this.loadedPosts.update(existing => {
                            return [...existing, ...newPosts];
                        });
                    }
                })
            );
        }
    });

    // Computed UI states
    isInitialLoading = computed(() => this.feedResource.isLoading() && this.loadedPosts().length === 0);
    isLoadingNextPage = computed(() => this.feedResource.isLoading() && this.loadedPosts().length > 0);
    hasError = computed(() => !this.feedResource.isLoading() && !!this.feedResource.error());
    isInitialError = computed(() => this.hasError() && this.loadedPosts().length === 0);
    isNextPageError = computed(() => this.hasError() && this.loadedPosts().length > 0);
    isEmptyFeed = computed(() => !this.feedResource.isLoading() && !this.hasError() && this.loadedPosts().length === 0);
    isEndOfFeed = computed(() => !this.hasNextPage() && this.loadedPosts().length > 0 && !this.feedResource.isLoading() && !this.hasError());

    // Sentinel for Infinite Scrolling
    private sentinel = viewChild<ElementRef<HTMLDivElement>>('sentinel');
    private observer: IntersectionObserver | null = null;

    constructor() {
        // Setup IntersectionObserver for sentinel when it enters/leaves DOM
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
                            this.loadNextPage();
                        }
                    },
                    {
                        root: null,
                        rootMargin: '0px 0px 400px 0px', // Trigger before reaching bottom
                        threshold: 0.1
                    }
                );
                this.observer.observe(sentinelEl);
            }
        });

        // Scroll Restoration when returning to Feed root route
        effect(() => {
            if (this.isFeedRootRoute() && this.savedScrollPosition > 0) {
                const scrollY = this.savedScrollPosition;
                setTimeout(() => {
                    window.scrollTo({ top: scrollY, behavior: 'instant' });
                }, 0);
            }
        });

        this.destroyRef.onDestroy(() => {
            if (this.observer) {
                this.observer.disconnect();
                this.observer = null;
            }
        });
    }

    private checkIsFeedRoot(rawUrl: string): boolean {
        const url = rawUrl.split('?')[0].split('#')[0];
        return url === '/home' || url === '/home/posts' || url === '/feed';
    }

    // Guarded page advancement
    loadNextPage(): void {
        if (this.feedResource.isLoading()) return;
        if (!this.hasNextPage()) return;
        if (this.hasError()) return;

        this.currentPage.update(page => page + 1);
    }

    // Retries the current page request using rxResource
    retry(): void {
        this.feedResource.reload();
    }

    onPostDeleted(postId: string): void {
        this.loadedPosts.update(posts => posts.filter(p => p.id !== postId));
    }

    onPostNotInterested(postId: string): void {
        this.dismissedPostIds.update(ids => {
            const next = new Set(ids);
            next.add(postId);
            return next;
        });
    }

    undoNotInterested(postId: string): void {
        this.dismissedPostIds.update(ids => {
            const next = new Set(ids);
            next.delete(postId);
            return next;
        });
        this.postsService.increaseInterest(postId).subscribe();
    }

    onPostCommentsClick(postId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate(['post', postId], { relativeTo: this.route });
    }
}
