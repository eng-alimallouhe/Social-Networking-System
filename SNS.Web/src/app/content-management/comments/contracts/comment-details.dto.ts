import { CommentSummaryDto } from './comment-summary.dto';

export interface CommentDetailsDto {
    comment: CommentSummaryDto;
    parentComment?: CommentSummaryDto | null;
    parentHasParent: boolean;
}
