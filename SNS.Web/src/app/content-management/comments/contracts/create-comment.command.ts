export interface CreateCommentCommand {
    postId: string;
    parentCommentId?: string | null;
    content: string;
    mentionedProfileIds?: string[] | null;
}
