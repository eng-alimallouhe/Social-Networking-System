import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideThumbsUp,
    LucideCheckCircle2,
    LucideEye,
    LucideCalendar,
    LucideFileCode,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideShare2,
    LucideBookmark,
    LucideVideo
} from '@lucide/angular';
import { ProblemsService } from '../../services/problems.service';
import { ProblemDetailsDto } from '../../contracts/problem-details.dto';
import { ProblemBlockType } from '../../../enums/problem-block-type.enum';
import { DifficultyLevel } from '../../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../../enums/problem-status.enum';
import { MarkdownService } from '../../../../../shared/services/markdown.service';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

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
        TranslatePipe,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideThumbsUp,
        LucideCheckCircle2,
        LucideEye,
        LucideCalendar,
        LucideFileCode,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideShare2,
        LucideBookmark,
        LucideVideo
    ],
    templateUrl: './problem-details.html',
    styleUrl: './problem-details.css'
})
export class ProblemDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private problemsService = inject(ProblemsService);
    private markdownService = inject(MarkdownService);

    readonly SkeletonType = SkeletonType;
    readonly ProblemBlockType = ProblemBlockType;
    readonly DifficultyLevel = DifficultyLevel;
    readonly ProblemStatus = ProblemStatus;
    readonly defaultAvatar = 'assets/images/default-avatar.png';

    problemId = signal<string>('');
    problem = signal<ProblemDetailsDto | null>(null);
    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);
    isSaved = signal<boolean>(false);

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
                    this.problem.set(res.value);
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

    toggleSave(): void {
        this.isSaved.update(s => !s);
    }

    share(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    onAvatarError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultAvatar) {
            target.src = this.defaultAvatar;
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
