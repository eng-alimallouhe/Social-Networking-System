using System.Linq.Expressions;

namespace SNS.Domain.Abstractions.Specifications
{
    public interface ISingleEntitySpecification<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the filtering criteria that will be applied to the query.
        /// Equivalent to a WHERE clause in SQL.
        /// </summary>
        Expression<Func<TEntity, bool>> Criteria { get; }

        /// <summary>
        /// Gets a collection of navigation property paths to be eagerly loaded.
        /// These paths are interpreted by the infrastructure layer.
        /// </summary>
        List<string> Includes { get; }
    }
}
