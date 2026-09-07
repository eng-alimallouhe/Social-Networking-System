import { Component, signal, inject, effect, computed, viewChild, ElementRef, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute, NavigationEnd, RouterOutlet } from '@angular/router';
import { map, tap, filter } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideRefreshCw, LucideAlertCircle, LucideMessagesSquare } from '@lucide/angular';
import { ProblemsService } from '../../services/problems.service';
import { ProblemSummaryDto } from '../../contracts/problem-summary.dto';
import { Problem } from '../problem/problem';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-problems-page',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        Problem,
        SkeletonLoaderComponent,
        RouterOutlet,
        LucideRefreshCw,
        LucideAlertCircle,
        LucideMessagesSquare
    ],
    templateUrl: './problems-page.html',
    styleUrl: './problems-page.css'
})
export class ProblemsPage {
    private problemsService = inject(ProblemsService);
    private destroyRef = inject(DestroyRef);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    readonly SkeletonType = SkeletonType;

    // Route State
    readonly isProblemsRootRoute = toSignal(
        this.router.events.pipe(
            filter((event): event is NavigationEnd => event instanceof NavigationEnd),
            map(event => this.checkIsProblemsRoot(event.urlAfterRedirects))
        ),
        {
            initialValue: this.checkIsProblemsRoot(this.router.url)
        }
    );

    private savedScrollPosition = 0;

    // Pagination & Feed State
    currentPage = signal<number>(1);
    pageSize = signal<number>(10);
    hasNextPage = signal<boolean>(true);
    loadedProblems = signal<ProblemSummaryDto[]>([]);

    // rxResource manages the HTTP request for the current page
    problemsResource = rxResource({
        params: () => ({
            page: this.currentPage(),
            size: this.pageSize()
        }),
        stream: ({ params }) => {
            return this.problemsService.getProblems(params.page, params.size).pipe(
                map(result => {
                    if (result && !result.isSuccess) {
                        throw result;
                    }
                    console.log(result);
                    return result;
                }),
                tap(result => {
                    if (!result?.isSuccess || !result.value) return;
                    const paged = result.value;
                    const newItems = paged.items || [];

                    this.hasNextPage.set(paged.hasNext);

                    if (newItems.length > 0) {
                        this.loadedProblems.update(existing => {
                            const existingIds = new Set(existing.map(p => p.id));
                            const uniqueNew = newItems.filter(p => !existingIds.has(p.id));
                            return [...existing, ...uniqueNew];
                        });
                    }
                })
            );
        }
    });

    // Computed UI states
    isInitialLoading = computed(() => this.problemsResource.isLoading() && this.loadedProblems().length === 0);
    isLoadingNextPage = computed(() => this.problemsResource.isLoading() && this.loadedProblems().length > 0);
    hasError = computed(() => !this.problemsResource.isLoading() && !!this.problemsResource.error());
    isInitialError = computed(() => this.hasError() && this.loadedProblems().length === 0);
    isNextPageError = computed(() => this.hasError() && this.loadedProblems().length > 0);
    isEmpty = computed(() => !this.problemsResource.isLoading() && !this.hasError() && this.loadedProblems().length === 0);
    isEndOfFeed = computed(() => !this.hasNextPage() && this.loadedProblems().length > 0 && !this.problemsResource.isLoading() && !this.hasError());

    // Sentinel for Infinite Scrolling
    private sentinel = viewChild<ElementRef<HTMLDivElement>>('sentinel');
    private observer: IntersectionObserver | null = null;

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
                            this.loadNextPage();
                        }
                    },
                    {
                        root: null,
                        rootMargin: '0px 0px 400px 0px',
                        threshold: 0.1
                    }
                );
                this.observer.observe(sentinelEl);
            }
        });

        // Scroll Restoration when returning to Problems root route
        effect(() => {
            if (this.isProblemsRootRoute() && this.savedScrollPosition > 0) {
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

    private checkIsProblemsRoot(rawUrl: string): boolean {
        const url = rawUrl.split('?')[0].split('#')[0].replace(/\/+$/, '');
        return url === '/home/problems' || url === '/home/discussion' || url === '/problems' || url === '/discussions';
    }

    loadNextPage(): void {
        if (this.problemsResource.isLoading()) return;
        if (!this.hasNextPage()) return;
        if (this.hasError()) return;

        this.currentPage.update(page => page + 1);
    }

    retry(): void {
        this.problemsResource.reload();
    }

    onProblemClick(problemId: string): void {
        this.savedScrollPosition = window.scrollY;
        this.router.navigate([problemId], { relativeTo: this.route });
    }
}
