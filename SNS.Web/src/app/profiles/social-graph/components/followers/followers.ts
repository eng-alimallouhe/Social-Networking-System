import { Component, OnInit, signal, computed, inject, viewChild, ElementRef, effect, DestroyRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideUsers,
    LucideAlertCircle,
    LucideRefreshCw
} from '@lucide/angular';
import { FollowsService } from '../../services/follows.service';
import { ProfileFollowDto } from '../../contracts/profile-follow.dto';
import { ProfileFollow } from '../profile-follow/profile-follow';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-followers',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        ProfileFollow,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideUsers,
        LucideAlertCircle,
        LucideRefreshCw
    ],
    templateUrl: './followers.html',
    styleUrl: './followers.css'
})
export class Followers implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private followsService = inject(FollowsService);
    private destroyRef = inject(DestroyRef);

    readonly SkeletonType = SkeletonType;

    // Profile Context
    profileId = signal<string>('');

    // Pagination & List State
    followers = signal<ProfileFollowDto[]>([]);
    currentPage = signal<number>(1);
    readonly pageSize = 10;
    hasNextPage = signal<boolean>(true);

    // Loading & Error States
    isLoading = signal<boolean>(true);
    isLoadingNextPage = signal<boolean>(false);
    hasError = signal<boolean>(false);
    isNextPageError = signal<boolean>(false);

    isEmpty = computed(() => !this.isLoading() && !this.hasError() && this.followers().length === 0);
    isEndOfList = computed(() => !this.hasNextPage() && this.followers().length > 0 && !this.isLoading() && !this.isLoadingNextPage() && !this.hasError());

    // Sentinel for Infinite Scrolling
    private sentinel = viewChild<ElementRef<HTMLDivElement>>('sentinel');
    private observer: IntersectionObserver | null = null;

    constructor() {
        // Setup IntersectionObserver for infinite scroll sentinel
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
                        rootMargin: '0px 0px 300px 0px',
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
            const id = params.get('profileId') || params.get('id');
            if (id && id !== this.profileId()) {
                this.profileId.set(id);
                this.loadFollowers(1, true);
            }
        });
    }

    loadFollowers(page: number = 1, isInitial: boolean = false): void {
        const id = this.profileId();
        if (!id) return;

        if (isInitial) {
            this.isLoading.set(true);
            this.hasError.set(false);
            this.followers.set([]);
            this.currentPage.set(1);
        } else {
            this.isLoadingNextPage.set(true);
            this.isNextPageError.set(false);
        }

        this.followsService.getProfileFollowers(id, page, this.pageSize).subscribe({
            next: res => {
                this.isLoading.set(false);
                this.isLoadingNextPage.set(false);

                if (res?.isSuccess && res.value) {
                    const paged = res.value;
                    const items = paged.items || [];

                    if (isInitial) {
                        this.followers.set(items);
                    } else {
                        // Append without duplicates
                        this.followers.update(existing => {
                            const existingIds = new Set(existing.map(p => p.profileId));
                            const uniqueNew = items.filter(p => !existingIds.has(p.profileId));
                            return [...existing, ...uniqueNew];
                        });
                    }

                    this.currentPage.set(page);
                    this.hasNextPage.set(paged.hasNext ?? (items.length >= this.pageSize));
                } else {
                    if (isInitial) {
                        this.hasError.set(true);
                    } else {
                        this.isNextPageError.set(true);
                    }
                }
            },
            error: () => {
                this.isLoading.set(false);
                this.isLoadingNextPage.set(false);

                if (isInitial) {
                    this.hasError.set(true);
                } else {
                    this.isNextPageError.set(true);
                }
            }
        });
    }

    loadNextPage(): void {
        if (this.isLoading() || this.isLoadingNextPage()) return;
        if (!this.hasNextPage()) return;
        if (this.hasError() || this.isNextPageError()) return;

        this.loadFollowers(this.currentPage() + 1, false);
    }

    retry(): void {
        if (this.isNextPageError()) {
            this.loadFollowers(this.currentPage() + 1, false);
        } else {
            this.loadFollowers(1, true);
        }
    }

    onProfileClicked(targetProfileId: string): void {
        const base = this.router.url.includes('/search/') ? '/home/search/profile' : '/home/profiles';
        this.router.navigate([base, targetProfileId]);
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            const base = this.router.url.includes('/search/') ? '/home/search/profile' : '/home/profiles';
            this.router.navigate([base, this.profileId()]);
        }
    }
}
