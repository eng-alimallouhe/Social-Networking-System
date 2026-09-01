export interface CreatePostCommand {
    communityId?: string | null;
    title: string;
    content: string;
    isPenned: boolean;
    files: any[];
    mentionedProfileIds?: string[];
}
