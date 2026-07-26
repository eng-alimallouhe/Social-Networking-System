using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.Identity.Users.Specifications;

public class UserWithRoleSpecification :
    ISingleEntitySpecification<User>
{
    public Expression<Func<User, bool>> Criteria { get; }

    public List<string> Includes { get; }

    public Expression<Func<User, object>>? OrderBy => null;

    public Expression<Func<User, object>>? OrderByDescending => null;

    public UserWithRoleSpecification(Guid userId)
    {
        Criteria = u => u.Id == userId;

        Includes = new List<string>()
        {
            "Role"
        };
    }
}
