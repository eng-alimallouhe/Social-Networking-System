export interface UpdatePostCommand {
    postId: string;
    title: string;
    content: string;
    deletedMediaIds: string[];
    newMedia: any[];
    deletedTagIds: string[];
    newTagIds: string[];
    mentionedProfileIds?: string[];
}
