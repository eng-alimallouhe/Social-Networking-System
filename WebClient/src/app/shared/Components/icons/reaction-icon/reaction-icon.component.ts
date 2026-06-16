import { Component, Input } from '@angular/core';
import { ReactionType } from '../../../../content/shared/enums/reactions-type.enum';
import { LucideHeart } from "@lucide/angular";

@Component({
  selector: 'app-reaction-icon',
  imports: [LucideHeart],
  templateUrl: './reaction-icon.component.html',
  styleUrl: './reaction-icon.component.css',
})
export class ReactionIconComponent {
  @Input({ required: true }) reactionType!: ReactionType;

  ReactionType = ReactionType;
}