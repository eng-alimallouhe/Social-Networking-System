import { Component, input, output, signal, inject, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideThumbsUp,
    LucideCornerUpLeft,
    LucideMoreHorizontal,
    LucideTrash2,
    LucideFlag
} from '@lucide/angular';
import { OverlayModule } from '@angular/cdk/overlay';
import { CommentSummaryDto } from '../../contracts/comment-summary.dto';
import { CommentReactionService } from '../../services/comment-reaction.service';
import { CommentsService } from '../../services/comments.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { ConfirmStateService } from '../../../../shared/design-system/services/confirm-state.service';
import { ConfirmAction } from '../../../../shared/design-system/services/confirm-action.enum';
import { AppConfirmDialog } from '../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { WantToLogin } from '../../../../shared/components/want-to-login/want-to-login';
import { CommentReport } from '../../../../moderation/components/comment-report/comment-report';
import { ReactionType } from '../../../../shared/contracts/reaction-type';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';

@Component({
    selector: 'app-comment',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        LocalDatePipe,
        OverlayModule,
        AppConfirmDialog,
        WantToLogin,
        CommentReport,
        LucideThumbsUp,
        LucideCornerUpLeft,
        LucideMoreHorizontal,
        LucideTrash2,
        LucideFlag
    ],
    templateUrl: './comment.html',
    styleUrls: ['./comment.css']
})
export class Comment {
    commentInput = input.required<CommentSummaryDto>({ alias: 'comment' });

    reply = output<CommentSummaryDto>();
    deleted = output<string>();

    // Local mutable state for optimistic reaction/comment updates
    commentState = signal<CommentSummaryDto | null>(null);

    activeComment = computed(() => this.commentState() ?? this.commentInput());

    isActionsMenuOpen = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);
    isDeleteConfirmOpen = signal<boolean>(false);
    isReportOpen = signal<boolean>(false);

    private reactionService = inject(CommentReactionService);
    private commentsService = inject(CommentsService);
    private authService = inject(AuthenticationService);
    private confirmStateService = inject(ConfirmStateService);
    private languageService = inject(LanguageService);

    readonly ReactionType = ReactionType;
    readonly ConfirmAction = ConfirmAction;

    constructor() {
        effect(() => {
            const action = this.confirmStateService.confirmedAction();
            if (action === ConfirmAction.Delete && this.isDeleteConfirmOpen()) {
                this.confirmStateService.consume();
                this.deleteComment();
            }
        });
    }

    isRtl = computed(() => {
        return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
            (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
    });

    currentProfileId = computed(() => this.authService.getClaim('ProfileId') || this.authService.getUserId());
    isOwner = computed(() => !!this.currentProfileId() && this.currentProfileId() === this.activeComment().author?.id);

    defaultAvatar = 'assets/images/default-avatar.png';

    toggleLike(): void {
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }

        const comment = this.activeComment();
        const isLiked = comment.currentUserReaction === ReactionType.Like;

        if (isLiked) {
            // Optimistic update
            this.commentState.set({
                ...comment,
                currentUserReaction: null,
                reactionsCount: Math.max(0, comment.reactionsCount - 1)
            });

            this.reactionService.removeReaction(comment.id).subscribe({
                error: () => this.commentState.set(comment) // Revert on failure
            });
        } else {
            // Optimistic update
            this.commentState.set({
                ...comment,
                currentUserReaction: ReactionType.Like,
                reactionsCount: comment.reactionsCount + 1
            });

            this.reactionService.addOrChangeReaction(comment.id, ReactionType.Like).subscribe({
                error: () => this.commentState.set(comment) // Revert on failure
            });
        }
    }

    onReply(): void {
        this.reply.emit(this.activeComment());
    }

    openDeleteConfirm(): void {
        this.isActionsMenuOpen.set(false);
        this.isDeleteConfirmOpen.set(true);
    }

    deleteComment(): void {
        const commentId = this.activeComment().id;
        this.commentsService.deleteComment(commentId).subscribe({
            next: () => {
                this.isDeleteConfirmOpen.set(false);
                this.deleted.emit(commentId);
            }
        });
    }

    openReport(): void {
        this.isActionsMenuOpen.set(false);
        if (!this.authService.isAuthenticated()) {
            this.isWantToLoginOpen.set(true);
            return;
        }
        this.isReportOpen.set(true);
    }
}
