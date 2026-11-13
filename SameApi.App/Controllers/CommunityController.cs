using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameApi.Business.Community.Command;
using SameApi.Business.Community.Query;

namespace SameApi.App.Controllers
{
    [ApiController]
    [Route("api/community")]
    [Authorize]
    public class CommunityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommunityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCommunityAsync([FromBody] CreateCommunityCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.Uid = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpPost("join/{slug}")]
        public async Task<IActionResult> Join(string slug)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            await _mediator.Send(new JoinCommunityCommand { Slug = slug, Uid = uid });
            return Ok();
        }

        [HttpPost("approve/{slug}")]
        public async Task<IActionResult> Approve(string slug)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            await _mediator.Send(new ApproveCommunityQuery { Slug = slug, Uid = uid });
            return Ok();
        }
    }
}