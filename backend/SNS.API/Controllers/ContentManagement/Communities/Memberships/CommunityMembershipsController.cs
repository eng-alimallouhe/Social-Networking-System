using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement.Communities;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.ApproveMembership;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.ChangeMemberRole;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.JoinCommunity;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.LeaveCommunity;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.RejectMembership;
using SNS.Application.ContentManagement.Communities.Memberships.Commands.RemoveMember;
using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.ContentManagement.Communities.Memberships.Queries.GetCommunityMembers;
using SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMembershipRequests;
using SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMyMembership;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagement.Communities.Memberships;

/// <summary>
/// Handles community membership management, join requests, approval workflows, and role administration.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/communities/{communityId:guid}/memberships")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunityMembershipsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunityMembershipsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Joins a public community directly or submits a membership request for a private community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="request">Optional application notes when joining a private community.</param>
    /// <response code="200">Joined or join request submitted successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="404">The community was not found.</response>
    /// <response code="409">User is already an active member or has an existing pending request.</response>
    [HttpPost("join")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result>> JoinCommunityAsync(
        [FromRoute] Guid communityId,
        [FromBody] JoinCommunityRequest? request)
    {
        return (await _mediator.Send(new JoinCommunityCommand(communityId, request?.Notes))).ToActionResult(this);
    }

    /// <summary>
    /// Leaves an existing community membership.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <response code="200">Left the community successfully.</response>
    /// <response code="400">Community owner cannot leave without transferring ownership.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="404">Membership not found.</response>
    [HttpPost("leave")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> LeaveCommunityAsync([FromRoute] Guid communityId)
    {
        return (await _mediator.Send(new LeaveCommunityCommand(communityId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated active members of a community, optionally filtered by role.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="role">Optional community role filter.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <response code="200">Returns paginated member list <see cref="Paged{CommunityMemberDto}"/>.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("members")]
    [ProducesResponseType(typeof(Result<Paged<CommunityMemberDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<CommunityMemberDto>>>> GetCommunityMembersAsync(
        [FromRoute] Guid communityId,
        [FromQuery] CommunityRole? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return (await _mediator.Send(new GetCommunityMembersQuery(communityId, role, page, pageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated pending membership requests for community owners and moderators.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <response code="200">Returns paginated membership requests <see cref="Paged{MembershipRequestDto}"/>.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("requests")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<MembershipRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<MembershipRequestDto>>>> GetMembershipRequestsAsync(
        [FromRoute] Guid communityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return (await _mediator.Send(new GetMembershipRequestsQuery(communityId, page, pageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Approves a pending membership request and activates the user's membership.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="requestId">The unique identifier of the join request.</param>
    /// <response code="200">Membership request approved successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The join request was not found or is no longer pending.</response>
    [HttpPost("requests/{requestId:guid}/approve")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ApproveMembershipAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid requestId)
    {
        return (await _mediator.Send(new ApproveMembershipCommand(requestId))).ToActionResult(this);
    }

    /// <summary>
    /// Rejects a pending membership request.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="requestId">The unique identifier of the join request.</param>
    /// <response code="200">Membership request rejected successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks moderator/owner permissions.</response>
    /// <response code="404">The join request was not found or is no longer pending.</response>
    [HttpPost("requests/{requestId:guid}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RejectMembershipAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid requestId)
    {
        return (await _mediator.Send(new RejectMembershipCommand(requestId))).ToActionResult(this);
    }

    /// <summary>
    /// Removes a member from a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="memberProfileId">The profile identifier of the member to remove.</param>
    /// <response code="200">Member removed successfully.</response>
    /// <response code="400">Cannot remove community owner.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks permissions to remove the specified member.</response>
    /// <response code="404">Membership not found.</response>
    [HttpDelete("members/{memberProfileId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveMemberAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid memberProfileId)
    {
        return (await _mediator.Send(new RemoveMemberCommand(communityId, memberProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Changes the role of a community member.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="memberProfileId">The profile identifier of the member.</param>
    /// <param name="request">The payload containing the new role.</param>
    /// <response code="200">Role changed successfully.</response>
    /// <response code="400">Invalid operation.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks permissions to assign the specified role.</response>
    /// <response code="404">Membership not found.</response>
    [HttpPut("members/{memberProfileId:guid}/role")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ChangeMemberRoleAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid memberProfileId,
        [FromBody] ChangeMemberRoleRequest request)
    {
        return (await _mediator.Send(new ChangeMemberRoleCommand(communityId, memberProfileId, request.NewRole))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the current user's membership and request status for a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <response code="200">Returns user membership status <see cref="UserMembershipStatusDto"/>.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet("my-status")]
    [Authorize]
    [ProducesResponseType(typeof(Result<UserMembershipStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<UserMembershipStatusDto>>> GetMyMembershipStatusAsync([FromRoute] Guid communityId)
    {
        return (await _mediator.Send(new GetMyMembershipQuery(communityId))).ToActionResult(this);
    }
}
