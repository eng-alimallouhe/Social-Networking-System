export interface UpdateCommentCommand {
    commentId?: string;
    content: string;
    mentionedProfileIds?: string[] | null;
}
