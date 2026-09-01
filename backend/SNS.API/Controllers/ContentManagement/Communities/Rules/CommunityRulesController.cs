using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement.Communities;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Rules.Commands.CreateCommunityRule;
using SNS.Application.ContentManagement.Communities.Rules.Commands.DeleteCommunityRule;
using SNS.Application.ContentManagement.Communities.Rules.Commands.UpdateCommunityRule;
using SNS.Application.ContentManagement.Communities.Rules.Contracts;
using SNS.Application.ContentManagement.Communities.Rules.Queries.GetCommunityRules;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagement.Communities.Rules;

/// <summary>
/// Handles community rules configuration, listing, creation, modification, and deletion.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/communities/{communityId:guid}/rules")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunityRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunityRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all structured rules configured for a community ordered by sort order.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <response code="200">Returns list of community rules <see cref="List{CommunityRuleDto}"/>.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<CommunityRuleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<List<CommunityRuleDto>>>> GetRulesAsync([FromRoute] Guid communityId)
    {
        return (await _mediator.Send(new GetCommunityRulesQuery(communityId))).ToActionResult(this);
    }

    /// <summary>
    /// Creates a new rule within a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="request">The rule creation payload.</param>
    /// <response code="200">Rule created successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks owner or moderator permissions.</response>
    /// <response code="404">The community was not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> CreateRuleAsync(
        [FromRoute] Guid communityId,
        [FromBody] CreateCommunityRuleRequest request)
    {
        var command = new CreateCommunityRuleCommand(
            communityId,
            request.Title,
            request.Description,
            request.Order);

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing rule in a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="ruleId">The unique identifier of the rule to update.</param>
    /// <param name="request">The rule update payload.</param>
    /// <response code="200">Rule updated successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks owner or moderator permissions.</response>
    /// <response code="404">The rule was not found.</response>
    [HttpPut("{ruleId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateRuleAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid ruleId,
        [FromBody] UpdateCommunityRuleRequest request)
    {
        var command = new UpdateCommunityRuleCommand(
            ruleId,
            request.Title,
            request.Description,
            request.Order);

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes a rule from a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="ruleId">The unique identifier of the rule to delete.</param>
    /// <response code="200">Rule deleted successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks owner or moderator permissions.</response>
    /// <response code="404">The rule was not found.</response>
    [HttpDelete("{ruleId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> DeleteRuleAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid ruleId)
    {
        return (await _mediator.Send(new DeleteCommunityRuleCommand(ruleId))).ToActionResult(this);
    }
}
