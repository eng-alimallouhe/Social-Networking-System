import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideArrowUp,
    LucideArrowDown,
    LucideEye,
    LucideCalendar,
    LucideFileCode,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideShare2,
    LucideBookmark,
    LucideVideo,
    LucideUsers,
    LucideLightbulb
} from '@lucide/angular';
import { ProblemsService } from '../../services/problems.service';
import { ProblemDetailsDto } from '../../contracts/problem-details.dto';
import { ProblemBlockType } from '../../../enums/problem-block-type.enum';
import { DifficultyLevel } from '../../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../../enums/problem-status.enum';
import { VoteType } from '../../../../shared/enums/vote-type.enum';
import { MarkdownService } from '../../../../../shared/services/markdown.service';
import { ProblemVotesService } from '../../../problem-votes/services/problem-votes.service';
import { SolutionsService } from '../../../../solutions/solutions/services/solutions.service';
import { SolutionSummaryDto } from '../../../../solutions/solutions/contracts/solution-summary.dto';
import { Solution } from '../../../../solutions/solutions/components/solution/solution';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';
import { AuthenticationService } from '../../../../../identity/shared/services/authentication.service';
import { LocalDatePipe } from '../../../../../shared/pipes/local-date.pipe';
import { WantToLogin } from '../../../../../shared/components/want-to-login/want-to-login';

import { AppAvatar } from '../../../../../shared/design-system/components/app-avatar/app-avatar';

export interface RenderedContentBlock {
    id: string;
    type: ProblemBlockType;
    order: number;
    rawContent: string;
    renderedHtml: string | null;
    extraInfo: string | null;
}

@Component({
    selector: 'app-problem-details',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        LocalDatePipe,
        WantToLogin,
        AppAvatar,
        Solution,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideArrowUp,
        LucideArrowDown,
        LucideEye,
        LucideCalendar,
        LucideFileCode,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideShare2,
        LucideBookmark,
        LucideVideo,
        LucideUsers,
        LucideLightbulb
    ],
    templateUrl: './problem-details.html',
    styleUrl: './problem-details.css'
})
export class ProblemDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private problemsService = inject(ProblemsService);
    private problemVotesService = inject(ProblemVotesService);
    private solutionsService = inject(SolutionsService);
    private markdownService = inject(MarkdownService);
    private authService = inject(AuthenticationService);

    readonly SkeletonType = SkeletonType;
    readonly ProblemBlockType = ProblemBlockType;
    readonly DifficultyLevel = DifficultyLevel;
    readonly ProblemStatus = ProblemStatus;

    problemId = signal<string>('');
    problem = signal<ProblemDetailsDto | null>(null);
    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);
    isSaved = signal<boolean>(false);

    // Problem Voting state
    upvotesCount = signal<number>(0);
    downvotesCount = signal<number>(0);
    isUpVoted = signal<boolean>(false);
    isDownVoted = signal<boolean>(false);
    isVoting = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);

    // Solutions state
    solutions = signal<SolutionSummaryDto[]>([]);
    isSolutionsInitialLoading = signal<boolean>(false);
    isLoadingMoreSolutions = signal<boolean>(false);
    solutionsError = signal<boolean>(false);
    solutionsPage = signal<number>(1);
    solutionsPageSize = signal<number>(10);
    hasMoreSolutions = signal<boolean>(false);
    solutionsTotalCount = signal<number>(0);

    renderedBlocks = computed<RenderedContentBlock[]>(() => {
        const p = this.problem();
        if (!p || !p.contentBlocks) return [];

        return [...p.contentBlocks]
            .sort((a, b) => a.order - b.order)
            .map(block => {
                let renderedHtml: string | null = null;
                if (block.type === ProblemBlockType.Text && block.content) {
                    renderedHtml = this.markdownService.parse(block.content);
                }
                return {
                    id: block.id,
                    type: block.type,
                    order: block.order,
                    rawContent: block.content || '',
                    renderedHtml,
                    extraInfo: block.extraInfo
                };
            });
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('problemId') || params.get('id');
            if (id && id !== this.problemId()) {
                this.problemId.set(id);
                this.loadProblem();
                this.loadSolutions(1, false);
            }
        });
    }

    loadProblem(): void {
        const id = this.problemId();
        if (!id) return;

        this.isLoading.set(true);
        this.hasError.set(false);

        this.problemsService.getProblemById(id).subscribe({
            next: res => {
                this.isLoading.set(false);
                if (res?.isSuccess && res.value) {
                    const p = res.value;
                    this.problem.set(p);
                    this.upvotesCount.set(p.upvotesCount ?? 0);
                    this.downvotesCount.set(p.downvotesCount ?? 0);
                    this.isUpVoted.set(p.currentUserVote === VoteType.Upvote);
                    this.isDownVoted.set(p.currentUserVote === VoteType.Downvote);
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

    loadSolutions(page: number = 1, append: boolean = false): void {
        const id = this.problemId();
        if (!id) return;

        if (append) {
            this.isLoadingMoreSolutions.set(true);
        } else {
            this.isSolutionsInitialLoading.set(true);
            this.solutionsError.set(false);
        }

        this.solutionsService.getProblemSolutions(id, this.solutionsPageSize(), page).subscribe({
            next: res => {
                this.isSolutionsInitialLoading.set(false);
                this.isLoadingMoreSolutions.set(false);

                if (res?.isSuccess && res.value) {
                    const paged = res.value;
                    const items = paged.items || [];
                    if (append) {
                        this.solutions.update(curr => [...curr, ...items]);
                    } else {
                        this.solutions.set(items);
                    }
                    this.hasMoreSolutions.set(paged.hasNext);
                    this.solutionsTotalCount.set(paged.totalCount);
                    this.solutionsPage.set(page);
                } else {
                    if (!append) {
                        this.solutionsError.set(true);
                    }
                }
            },
            error: () => {
                this.isSolutionsInitialLoading.set(false);
                this.isLoadingMoreSolutions.set(false);
                if (!append) {
                    this.solutionsError.set(true);
                }
            }
        });
    }

    loadMoreSolutions(): void {
        if (this.isLoadingMoreSolutions() || !this.hasMoreSolutions()) return;
        this.loadSolutions(this.solutionsPage() + 1, true);
    }

    toggleUpvote(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }
        if (this.isVoting() || !this.problem()) return;

        const currentUp = this.isUpVoted();
        const currentDown = this.isDownVoted();
        const prevUp = this.upvotesCount();
        const prevDown = this.downvotesCount();

        this.isVoting.set(true);

        if (currentUp) {
            this.isUpVoted.set(false);
            this.upvotesCount.update(c => Math.max(0, c - 1));

            this.problemVotesService.removeVote(this.problem()!.id).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.isUpVoted.set(currentUp);
                        this.upvotesCount.set(prevUp);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.isUpVoted.set(currentUp);
                    this.upvotesCount.set(prevUp);
                }
            });
        } else {
            this.isUpVoted.set(true);
            this.upvotesCount.update(c => c + 1);
            if (currentDown) {
                this.isDownVoted.set(false);
                this.downvotesCount.update(c => Math.max(0, c - 1));
            }

            this.problemVotesService.addOrChangeVote(this.problem()!.id, { type: VoteType.Upvote }).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.isUpVoted.set(currentUp);
                        this.isDownVoted.set(currentDown);
                        this.upvotesCount.set(prevUp);
                        this.downvotesCount.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.isUpVoted.set(currentUp);
                    this.isDownVoted.set(currentDown);
                    this.upvotesCount.set(prevUp);
                    this.downvotesCount.set(prevDown);
                }
            });
        }
    }

    toggleDownvote(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }
        if (this.isVoting() || !this.problem()) return;

        const currentUp = this.isUpVoted();
        const currentDown = this.isDownVoted();
        const prevUp = this.upvotesCount();
        const prevDown = this.downvotesCount();

        this.isVoting.set(true);

        if (currentDown) {
            this.isDownVoted.set(false);
            this.downvotesCount.update(c => Math.max(0, c - 1));

            this.problemVotesService.removeVote(this.problem()!.id).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.isDownVoted.set(currentDown);
                        this.downvotesCount.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.isDownVoted.set(currentDown);
                    this.downvotesCount.set(prevDown);
                }
            });
        } else {
            this.isDownVoted.set(true);
            this.downvotesCount.update(c => c + 1);
            if (currentUp) {
                this.isUpVoted.set(false);
                this.upvotesCount.update(c => Math.max(0, c - 1));
            }

            this.problemVotesService.addOrChangeVote(this.problem()!.id, { type: VoteType.Downvote }).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.isUpVoted.set(currentUp);
                        this.isDownVoted.set(currentDown);
                        this.upvotesCount.set(prevUp);
                        this.downvotesCount.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.isUpVoted.set(currentUp);
                    this.isDownVoted.set(currentDown);
                    this.upvotesCount.set(prevUp);
                    this.downvotesCount.set(prevDown);
                }
            });
        }
    }

    toggleSave(): void {
        this.isSaved.update(s => !s);
    }

    share(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home/problems']);
        }
    }
}
