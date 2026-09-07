using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement.Communities;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Communities.Commands.CreateCommunity;
using SNS.Application.ContentManagement.Communities.Communities.Commands.DeleteCommunity;
using SNS.Application.ContentManagement.Communities.Communities.Commands.UpdateCommunity;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Communities.Communities.Queries.GetCommunityById;
using SNS.Application.ContentManagement.Communities.Communities.Queries.GetMyCommunities;
using SNS.Application.ContentManagement.Communities.Communities.Queries.GetSuggestedCommunities;
using SNS.Application.ContentManagement.Posts.Posts.Commands.ApproveCommunityPost;
using SNS.Application.ContentManagement.Posts.Posts.Commands.RejectCommunityPost;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetCommunityPosts;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetPendingCommunityPosts;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.ContentManagement.Communities.Communities;

/// <summary>
/// Handles community creation, profile-owned community retrieval, details inspection, and community updates.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/communities")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new community on the platform with initial settings and rules.
    /// </summary>
    /// <param name="request">The multipart form request containing community details, logo, and initial settings.</param>
    /// <response code="200">Community created successfully.</response>
    /// <response code="400">The provided request parameters are invalid.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="409">A community with the specified name already exists.</response>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [RequireSession]
    public async Task<ActionResult<Result>> CreateCommunityAsync([FromForm] CreateCommunityRequest request)
    {
        var command = new CreateCommunityCommand(
            Name: request.Name,
            Description: request.Description,
            RulesText: request.RulesText,
            Policy: request.Policy,
            Type: request.Type,
            Logo: request.Logo?.ToUploadedFile(),
            Settings: request.Settings,
            Rules: request.Rules);

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing community's profile information, status, moderation policy, or logo.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <param name="request">The multipart form request containing updated community details.</param>
    /// <response code="200">Community updated successfully.</response>
    /// <response code="400">The update payload is invalid.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks owner or moderator permissions for this community.</response>
    /// <response code="404">The community was not found.</response>
    /// <response code="409">The updated name conflicts with an existing community.</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateCommunityAsync(
        [FromRoute] Guid id,
        [FromForm] UpdateCommunityRequest request)
    {
        var command = new UpdateCommunityCommand(
            CommunityId: id,
            Name: request.Name,
            Description: request.Description,
            RulesText: request.RulesText,
            Policy: request.Policy,
            Type: request.Type,
            Status: request.Status,
            Logo: request.Logo?.ToUploadedFile());

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes an existing community.
    /// </summary>
    /// <param name="id">The unique identifier of the community to delete.</param>
    /// <response code="200">Community deleted successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User is not the owner of the community.</response>
    /// <response code="404">The community was not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteCommunityAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new DeleteCommunityCommand(id))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves full community profile details including membership state and counters.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <response code="200">Returns community details <see cref="CommunityDetailsDto"/>.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<CommunityDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CommunityDetailsDto>>> GetCommunityByIdAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new GetCommunityByIdQuery(id))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated communities where the current authenticated user is an owner or active member.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <response code="200">Returns paginated community list <see cref="Paged{CommunitySummaryDto}"/>.</response>
    /// <response code="401">User is unauthenticated.</response>
    [HttpGet("my-communities")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<CommunitySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<CommunitySummaryDto>>>> GetMyCommunitiesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetMyCommunitiesQuery(page, pageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves suggested/recommended communities for the authenticated user.
    /// </summary>
    /// <param name="count">Number of suggested communities to retrieve.</param>
    /// <response code="200">Returns list of recommended communities.</response>
    [HttpGet("suggested")]
    [ProducesResponseType(typeof(Result<List<CommunitySummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<CommunitySummaryDto>>>> GetSuggestedCommunitiesAsync(
        [FromQuery] int count = 5)
    {
        return (await _mediator.Send(new GetSuggestedCommunitiesQuery(count))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated active posts belonging to a community.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <response code="200">Returns paginated post list <see cref="Paged{PostOverviewDto}"/>.</response>
    /// <response code="401">User is unauthenticated (if private community).</response>
    /// <response code="403">User is unauthorized to access private community posts.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("{id:guid}/posts")]
    [ProducesResponseType(typeof(Result<Paged<PostOverviewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<PostOverviewDto>>>> GetCommunityPostsAsync(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetCommunityPostsQuery(id, page, pageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated posts awaiting approval in a community for community managers/moderators.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <response code="200">Returns paginated pending posts <see cref="Paged{PostOverviewDto}"/>.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("{id:guid}/pending-posts")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<PostOverviewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<PostOverviewDto>>>> GetPendingCommunityPostsAsync(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetPendingCommunityPostsQuery(id, page, pageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Approves a pending community post.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <param name="postId">The unique identifier of the post.</param>
    /// <response code="200">Post approved successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The post was not found or is no longer pending.</response>
    [HttpPost("{id:guid}/pending-posts/{postId:guid}/approve")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> ApproveCommunityPostAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid postId)
    {
        return (await _mediator.Send(new ApproveCommunityPostCommand(id, postId))).ToActionResult(this);
    }

    /// <summary>
    /// Rejects a pending community post.
    /// </summary>
    /// <param name="id">The unique identifier of the community.</param>
    /// <param name="postId">The unique identifier of the post.</param>
    /// <response code="200">Post rejected successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The post was not found or is no longer pending.</response>
    [HttpPost("{id:guid}/pending-posts/{postId:guid}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RejectCommunityPostAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid postId)
    {
        return (await _mediator.Send(new RejectCommunityPostCommand(id, postId))).ToActionResult(this);
    }
}
