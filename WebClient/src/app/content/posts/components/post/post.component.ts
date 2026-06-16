import { Component, Input, signal, HostListener, ElementRef, inject } from '@angular/core';
import { ReactionType } from '../../../shared/enums/reactions-type.enum';
import { PostSummaryDto } from '../../dtos/post-summary.dto';
import { DecimalPipe, CommonModule } from '@angular/common';
import { LucideAArrowDown, LucideTrendingUp, LucideMoreVertical, LucideEdit2, LucideTrash2, LucideFlag, LucideBookmark, LucideCopy, LucideEyeOff, LucideSave, LucideHeart, LucideFlipVertical2, LucideEllipsisVertical } from "@lucide/angular";
import { ReactionIconComponent } from "../../../../shared/Components/icons/reaction-icon/reaction-icon.component";
import { PostReactionService } from '../../services/post-reaction.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { catchError, EMPTY, of } from 'rxjs';
import { DateConverterPipe } from "../../../../shared/Pipes/date-converter-pipe";
import { Result } from '../../../../shared/dtos/result.dto';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-post',
  imports: [
    DecimalPipe,
    CommonModule,
    TranslatePipe,
    LucideTrendingUp,
    LucideFlipVertical2,
    LucideSave,
    ReactionIconComponent,
    DateConverterPipe,
    LucideEllipsisVertical
],
  templateUrl: './post.component.html',
  styleUrl: './post.component.css',
})
export class PostComponent {
  @Input({ required: true }) post!: PostSummaryDto;
  @Input({ required: true }) profileId?: string | null;

  private postReactionService = inject(PostReactionService);
  private translateService = inject(TranslateService);
  private toastService = inject(ToastService);

  private elementRef = inject(ElementRef);
  public isMenuOpen = signal(false);

  private wasLongPress = false;

  public isReactionMenuOpen = signal(false);

  private hoverTimeout: any;
  private closeTimeout: any;
  private touchTimeout: any;

  constructor() {
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isMenuOpen.set(false);
    }
  }

  toggleMenu(event: MouseEvent) {
    event.stopPropagation();
    this.isMenuOpen.update(v => !v);
  }

  closeMenu() {
    this.isMenuOpen.set(false);
  }

  ReactionType = ReactionType;

  get reactionLabel(): string {
    switch (this.post.currentUserReactionType) {
      case ReactionType.Like: return 'Content.Reactions.Liked';
      case ReactionType.Love: return 'Content.Reactions.Loved';
      case ReactionType.Haha: return 'Content.Reactions.Haha';
      case ReactionType.Wow: return 'Content.Reactions.Wow';
      case ReactionType.Sad: return 'Content.Reactions.Sad';
      case ReactionType.Angry: return 'Content.Reactions.Angry';
      case ReactionType.Disgust: return 'Content.Reactions.Disgust';
      case ReactionType.Support: return 'Content.Reactions.Support';
      default: return 'Content.Reactions.Like';
    }
  }

  get reactionIcon(): string {
    switch (this.post.currentUserReactionType) {
      case ReactionType.Like: return 'thumb_up';
      case ReactionType.Love: return 'favorite';
      case ReactionType.Haha: return 'mood';
      case ReactionType.Wow: return 'error_outline';
      case ReactionType.Sad: return 'sentiment_very_dissatisfied';
      case ReactionType.Angry: return 'mood_bad';
      default: return 'thumb_up_off_alt';
    }
  }

  isCurrentReaction(reaction: ReactionType): boolean {
    return reaction === this.post.currentUserReactionType;
  }

  onReactionClick(reaction: ReactionType) {
    if (reaction === this.post.currentUserReactionType) {
      this.post.currentUserReactionType = ReactionType.None;
      this.post.reactionsCount--;
      this.postReactionService.deleteReaction(this.post.id).subscribe({
        next: () => {

        },
        error: () => {
          this.post.currentUserReactionType = reaction;
          this.post.reactionsCount++;
        }
      });
    }
    else {
      this.post.currentUserReactionType = reaction;
      this.post.reactionsCount++;
    }
  }

  onMouseEnter() {
    clearTimeout(this.closeTimeout);
    this.hoverTimeout = setTimeout(() => {
      this.isReactionMenuOpen.set(true);
    }, 400);
  }

  onMouseLeave() {
    clearTimeout(this.hoverTimeout);
    this.closeTimeout = setTimeout(() => {
      this.isReactionMenuOpen.set(false);
    }, 400);
  }

  onTouchStart(event: TouchEvent) {
    this.wasLongPress = false;

    this.touchTimeout = setTimeout(() => {
      this.wasLongPress = true;
      this.isReactionMenuOpen.set(true);

      if (navigator.vibrate) navigator.vibrate(50);
    }, 500);
  }

  onTouchEnd() {
    clearTimeout(this.touchTimeout);
  }

  onLikeClick(event: Event) {
    if (this.wasLongPress) {
      this.wasLongPress = false;
      return;
    }

    if (this.isReactionMenuOpen()) {
      this.isReactionMenuOpen.set(false);
      return;
    }

    this.toggleDefaultReaction();
  }

  toggleDefaultReaction() {
    if (this.post.currentUserReactionType == ReactionType.None) {
      this.post.currentUserReactionType = ReactionType.Like;
      this.post.reactionsCount++;
      this.postReactionService.createReaction(this.post.id, ReactionType.Like)
        .pipe(
          catchError((error: HttpErrorResponse) => {

            this.rollbackReaction();

            const result = error.error as Result<void>;

            const key = result?.statusCode?.category + '.' + result?.statusCode?.code;

            const title = this.translateService.instant('App.Messages.Error_Title');
            const message = this.translateService.instant(key);

            this.toastService.error(title, message);

            return EMPTY;
          })
        )
        .subscribe();
    }
    else {
      this.post.currentUserReactionType = ReactionType.None;
      this.post.reactionsCount--;
      this.postReactionService.deleteReaction(this.post.id)
        .pipe(
          catchError((error: HttpErrorResponse) => {
            this.rollbackReaction();

            const result = error.error as Result<void>;

            const key = result?.statusCode?.category + '.' + result?.statusCode?.code;

            const title = this.translateService.instant('App.Messages.Error_Title');
            const message = this.translateService.instant(key);

            this.toastService.error(title, message);

            return EMPTY;
          })
        )
        .subscribe();
    }
  }


  private rollbackReaction() {
    this.post.currentUserReactionType = ReactionType.None;
    this.post.reactionsCount--;
  }

}
