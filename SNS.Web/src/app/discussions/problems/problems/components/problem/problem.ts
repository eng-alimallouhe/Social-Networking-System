import { Component, input, output, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowUp,
    LucideArrowDown,
    LucideLightbulb,
    LucideBookmark,
    LucideClock,
    LucideUsers,
    LucideVideo
} from '@lucide/angular';
import { ProblemSummaryDto } from '../../contracts/problem-summary.dto';
import { DifficultyLevel } from '../../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../../enums/problem-status.enum';
import { ProblemBlockType } from '../../../enums/problem-block-type.enum';
import { VoteType } from '../../../../shared/enums/vote-type.enum';
import { MarkdownService } from '../../../../../shared/services/markdown.service';
import { ProblemVotesService } from '../../../problem-votes/services/problem-votes.service';
import { AuthenticationService } from '../../../../../identity/shared/services/authentication.service';
import { LocalDatePipe } from '../../../../../shared/pipes/local-date.pipe';
import { WantToLogin } from '../../../../../shared/components/want-to-login/want-to-login';

import { AppAvatar } from '../../../../../shared/design-system/components/app-avatar/app-avatar';

export interface ProblemPreviewBlock {
    id: string;
    type: ProblemBlockType;
    order: number;
    renderedHtml: string | null;
    rawContent: string;
    extraInfo: string | null;
}

@Component({
    selector: 'app-problem',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        LocalDatePipe,
        WantToLogin,
        AppAvatar,
        LucideArrowUp,
        LucideArrowDown,
        LucideLightbulb,
        LucideBookmark,
        LucideClock,
        LucideUsers,
        LucideVideo
    ],
    templateUrl: './problem.html',
    styleUrl: './problem.css'
})
export class Problem {
    private markdownService = inject(MarkdownService);
    private problemVotesService = inject(ProblemVotesService);
    private authService = inject(AuthenticationService);

    problem = input.required<ProblemSummaryDto>();
    problemClicked = output<string>();

    readonly DifficultyLevel = DifficultyLevel;
    readonly ProblemStatus = ProblemStatus;
    readonly ProblemBlockType = ProblemBlockType;

    // Reactive Local Overrides for voting (computed from input with local override support)
    private localUpvotes = signal<number | null>(null);
    private localDownvotes = signal<number | null>(null);
    private localIsUpVoted = signal<boolean | null>(null);
    private localIsDownVoted = signal<boolean | null>(null);

    upvotesCount = computed(() => this.localUpvotes() ?? this.problem()?.upvotesCount ?? 0);
    downvotesCount = computed(() => this.localDownvotes() ?? this.problem()?.downvotesCount ?? 0);
    isUpVoted = computed(() => this.localIsUpVoted() ?? this.problem()?.isUpVotedByCurrentUser ?? false);
    isDownVoted = computed(() => this.localIsDownVoted() ?? this.problem()?.isDownVotedByCurrentUser ?? false);

    isSaved = signal<boolean>(false);
    isVoting = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);

    onProblemClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.problemClicked.emit(this.problem().id);
    }

    toggleUpvote(event: Event): void {
        event.stopPropagation();
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
            // Remove upvote
            this.localIsUpVoted.set(false);
            this.localUpvotes.set(Math.max(0, prevUp - 1));

            this.problemVotesService.removeVote(this.problem().id).subscribe({
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
            // Add upvote (and remove downvote if active)
            this.localIsUpVoted.set(true);
            this.localUpvotes.set(prevUp + 1);
            if (currentDown) {
                this.localIsDownVoted.set(false);
                this.localDownvotes.set(Math.max(0, prevDown - 1));
            }

            this.problemVotesService.addOrChangeVote(this.problem().id, { type: VoteType.Upvote }).subscribe({
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

    toggleDownvote(event: Event): void {
        event.stopPropagation();
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
            // Remove downvote
            this.localIsDownVoted.set(false);
            this.localDownvotes.set(Math.max(0, prevDown - 1));

            this.problemVotesService.removeVote(this.problem().id).subscribe({
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
            // Add downvote (and remove upvote if active)
            this.localIsDownVoted.set(true);
            this.localDownvotes.set(prevDown + 1);
            if (currentUp) {
                this.localIsUpVoted.set(false);
                this.localUpvotes.set(Math.max(0, prevUp - 1));
            }

            this.problemVotesService.addOrChangeVote(this.problem().id, { type: VoteType.Downvote }).subscribe({
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

    toggleSave(event: Event): void {
        event.stopPropagation();
        this.isSaved.update(s => !s);
    }

    authorInitials = computed(() => {
        const name = this.problem()?.author?.fullName?.trim();
        if (!name) return '??';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    previewBlocks = computed<ProblemPreviewBlock[]>(() => {
        const blocks = (this.problem()?.contentBlocks || [])
            .filter(b => b.content && b.content.trim().length > 0)
            .sort((a, b) => a.order - b.order);

        if (!blocks.length) return [];

        return blocks.slice(0, 2).map((block, index) => {
            let renderedHtml: string | null = null;
            if (block.type === ProblemBlockType.Text) {
                const maxLen = index === 0 ? 250 : 120;
                const truncated = this.markdownService.parseAndTruncate(block.content, maxLen);
                renderedHtml = truncated.html;
            }
            return {
                id: block.id,
                type: block.type,
                order: block.order,
                renderedHtml,
                rawContent: block.content || '',
                extraInfo: block.extraInfo
            };
        });
    });
}
