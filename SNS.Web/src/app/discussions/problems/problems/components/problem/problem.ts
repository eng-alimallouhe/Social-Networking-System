import { Component, input, output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideThumbsUp,
    LucideCheckCircle2,
    LucideCalendar,
    LucideArrowRight,
    LucideVideo
} from '@lucide/angular';
import { ProblemSummaryDto } from '../../contracts/problem-summary.dto';
import { DifficultyLevel } from '../../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../../enums/problem-status.enum';
import { ProblemBlockType } from '../../../enums/problem-block-type.enum';
import { MarkdownService } from '../../../../../shared/services/markdown.service';

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
        LucideThumbsUp,
        LucideCheckCircle2,
        LucideCalendar,
        LucideArrowRight,
        LucideVideo
    ],
    templateUrl: './problem.html',
    styleUrl: './problem.css'
})
export class Problem {
    private markdownService = inject(MarkdownService);

    problem = input.required<ProblemSummaryDto>();
    problemClicked = output<string>();

    readonly DifficultyLevel = DifficultyLevel;
    readonly ProblemStatus = ProblemStatus;
    readonly ProblemBlockType = ProblemBlockType;
    readonly defaultAvatar = 'assets/images/default-avatar.png';

    onProblemClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.problemClicked.emit(this.problem().id);
    }

    onAvatarError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultAvatar) {
            target.src = this.defaultAvatar;
        }
    }

    authorInitials = computed(() => {
        const name = this.problem().authorName?.trim();
        if (!name) return '??';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    previewBlocks = computed<ProblemPreviewBlock[]>(() => {
        const blocks = (this.problem().contentBlocks || [])
            .filter(b => b.content && b.content.trim().length > 0)
            .sort((a, b) => a.order - b.order);

        if (!blocks.length) return [];

        // Return controlled preview of at most 2 blocks (Block 1 visible, Block 2 truncated)
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
