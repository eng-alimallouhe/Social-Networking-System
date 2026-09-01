using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.ContentManagement.Posts.Specifications;

public class PostToUpdateSpecification
    : ISingleEntitySpecification<Post>
{
    public Expression<Func<Post, bool>> Criteria  { get; }

    public List<string> Includes => [];

    public Expression<Func<Post, object>>? OrderBy => null;

    public Expression<Func<Post, object>>? OrderByDescending => null;

    public PostToUpdateSpecification(Guid id)
    {
        Criteria = p => p.Id == id;

        Includes.Add(nameof(Post.Media));
        Includes.Add(nameof(Post.PostTags));
        Includes.Add("PostTags.Tag");
        Includes.Add(nameof(Post.Mentions));
    }
}