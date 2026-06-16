using SNS.Shared.Results;
using MediatR;

namespace SNS.Application.Shared.Abstractions.Messaging;

/// <summary>
/// Represents a query request that returns a successful result with data.
/// Queries are strictly read-only and must not alter the system state.
/// </summary>
/// <typeparam name="TResponse">The type of data returned upon successful execution.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// Defines the contract for handling a specific query.
/// </summary>
/// <typeparam name="TQuery">The type of query to be handled.</typeparam>
/// <typeparam name="TResponse">The type of data returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
