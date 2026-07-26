using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.ContentManagement.Posts.Queries.GetFeed;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagment;

[Route("api/v{version:apiVersion}/content-managment/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("feed")]
    public async Task<ActionResult<Result<List<PostOverviewDto>>>> GetFeedAsync()
    {
        return (await _mediator.Send(new GetFeedQuery())).ToActionResult(this);
    }


    [HttpPost]
    public async Task<ActionResult<Result<List<PostOverviewDto>>>> CreatePostAsync()
    {
        return (await _mediator.Send(new GetFeedQuery())).ToActionResult(this);
    }


    [HttpPut]
    public async Task<ActionResult<Result<List<PostOverviewDto>>>> UpdatePostAsync()
    {
        return (await _mediator.Send(new GetFeedQuery())).ToActionResult(this);
    }


}
