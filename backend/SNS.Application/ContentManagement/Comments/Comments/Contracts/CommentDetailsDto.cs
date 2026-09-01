namespace SNS.Application.ContentManagement.Comments.Comments.Contracts;

public sealed record CommentDetailsDto(
    CommentSummaryDto Comment,
    CommentSummaryDto? ParentComment,
    bool ParentHasParent
);
