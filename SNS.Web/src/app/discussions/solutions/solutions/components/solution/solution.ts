import { Component, input, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowUp,
    LucideArrowDown,
    LucideMessagesSquare,
    LucideCheckCircle2,
    LucideClock,
    LucideHourglass,
    LucideBookmark,
    LucideShare2,
    LucideFileCode
} from '@lucide/angular';
import { SolutionSummaryDto } from '../../contracts/solution-summary.dto';
import { SolutionStatus } from '../../../enums/solution-status.enum';
import { SolutionBlockType } from '../../../enums/solution-block-type.enum';
import { VoteType } from '../../../../shared/enums/vote-type.enum';
import { MarkdownService } from '../../../../../shared/services/markdown.service';
import { SolutionVotesService } from '../../../solution-votes/services/solution-votes.service';
import { AuthenticationService } from '../../../../../identity/shared/services/authentication.service';
import { LocalDatePipe } from '../../../../../shared/pipes/local-date.pipe';
import { WantToLogin } from '../../../../../shared/components/want-to-login/want-to-login';

import { AppAvatar } from '../../../../../shared/design-system/components/app-avatar/app-avatar';

export interface RenderedSolutionBlock {
    id: string;
    type: SolutionBlockType;
    order: number;
    rawContent: string;
    renderedHtml: string | null;
    extraInfo: string | null;
}

@Component({
    selector: 'app-solution',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        LocalDatePipe,
        WantToLogin,
        AppAvatar,
        LucideArrowUp,
        LucideArrowDown,
        LucideMessagesSquare,
        LucideCheckCircle2,
        LucideClock,
        LucideHourglass,
        LucideBookmark,
        LucideShare2,
        LucideFileCode
    ],
    templateUrl: './solution.html',
    styleUrl: './solution.css'
})
export class Solution {
    private markdownService = inject(MarkdownService);
    private solutionVotesService = inject(SolutionVotesService);
    private authService = inject(AuthenticationService);

    solution = input.required<SolutionSummaryDto>();

    readonly SolutionStatus = SolutionStatus;
    readonly SolutionBlockType = SolutionBlockType;

    // Local overrides for optimistic voting
    private localUpvotes = signal<number | null>(null);
    private localDownvotes = signal<number | null>(null);
    private localIsUpVoted = signal<boolean | null>(null);
    private localIsDownVoted = signal<boolean | null>(null);

    upvotesCount = computed(() => this.localUpvotes() ?? this.solution()?.upvotesCount ?? 0);
    downvotesCount = computed(() => this.localDownvotes() ?? this.solution()?.downvotesCount ?? 0);
    isUpVoted = computed(() => this.localIsUpVoted() ?? (this.solution()?.isUpVotedByCurrentUser || this.solution()?.currentUserVote === VoteType.Upvote));
    isDownVoted = computed(() => this.localIsDownVoted() ?? (this.solution()?.isDownVotedByCurrentUser || this.solution()?.currentUserVote === VoteType.Downvote));

    isSaved = signal<boolean>(false);
    isVoting = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);

    authorInitials = computed(() => {
        const name = this.solution()?.author?.fullName?.trim();
        if (!name) return '??';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    renderedBlocks = computed<RenderedSolutionBlock[]>(() => {
        const s = this.solution();
        if (!s?.contentBlocks || !s.contentBlocks.length) return [];

        return [...s.contentBlocks]
            .sort((a, b) => a.order - b.order)
            .map(block => {
                let renderedHtml: string | null = null;
                if (block.type === SolutionBlockType.Text && block.content) {
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

    toggleUpvote(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }
        if (this.isVoting()) return;

        const currentUp = this.isUpVoted();
        const currentDown = this.isDownVoted();
        const prevUp = this.upvotesCount();
        const prevDown = this.downvotesCount();

        this.isVoting.set(true);

        if (currentUp) {
            this.localIsUpVoted.set(false);
            this.localUpvotes.set(Math.max(0, prevUp - 1));

            this.solutionVotesService.removeVote(this.solution().id).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.localIsUpVoted.set(currentUp);
                        this.localUpvotes.set(prevUp);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.localIsUpVoted.set(currentUp);
                    this.localUpvotes.set(prevUp);
                }
            });
        } else {
            this.localIsUpVoted.set(true);
            this.localUpvotes.set(prevUp + 1);
            if (currentDown) {
                this.localIsDownVoted.set(false);
                this.localDownvotes.set(Math.max(0, prevDown - 1));
            }

            this.solutionVotesService.addOrChangeVote(this.solution().id, { type: VoteType.Upvote }).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.localIsUpVoted.set(currentUp);
                        this.localIsDownVoted.set(currentDown);
                        this.localUpvotes.set(prevUp);
                        this.localDownvotes.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.localIsUpVoted.set(currentUp);
                    this.localIsDownVoted.set(currentDown);
                    this.localUpvotes.set(prevUp);
                    this.localDownvotes.set(prevDown);
                }
            });
        }
    }

    toggleDownvote(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }
        if (this.isVoting()) return;

        const currentUp = this.isUpVoted();
        const currentDown = this.isDownVoted();
        const prevUp = this.upvotesCount();
        const prevDown = this.downvotesCount();

        this.isVoting.set(true);

        if (currentDown) {
            this.localIsDownVoted.set(false);
            this.localDownvotes.set(Math.max(0, prevDown - 1));

            this.solutionVotesService.removeVote(this.solution().id).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.localIsDownVoted.set(currentDown);
                        this.localDownvotes.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.localIsDownVoted.set(currentDown);
                    this.localDownvotes.set(prevDown);
                }
            });
        } else {
            this.localIsDownVoted.set(true);
            this.localDownvotes.set(prevDown + 1);
            if (currentUp) {
                this.localIsUpVoted.set(false);
                this.localUpvotes.set(Math.max(0, prevUp - 1));
            }

            this.solutionVotesService.addOrChangeVote(this.solution().id, { type: VoteType.Downvote }).subscribe({
                next: res => {
                    this.isVoting.set(false);
                    if (!res.isSuccess) {
                        this.localIsUpVoted.set(currentUp);
                        this.localIsDownVoted.set(currentDown);
                        this.localUpvotes.set(prevUp);
                        this.localDownvotes.set(prevDown);
                    }
                },
                error: () => {
                    this.isVoting.set(false);
                    this.localIsUpVoted.set(currentUp);
                    this.localIsDownVoted.set(currentDown);
                    this.localUpvotes.set(prevUp);
                    this.localDownvotes.set(prevDown);
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
}
