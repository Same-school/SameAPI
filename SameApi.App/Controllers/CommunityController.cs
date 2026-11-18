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

        [HttpPatch("delete")]
        public async Task<IActionResult> DeleteCommunity([FromBody] DeleteCommunityCommand command, CancellationToken ct)
        {
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

        [HttpPost("leave/{slug}")]
        public async Task<IActionResult> Leave(string slug)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            await _mediator.Send(new LeaveCommunityCommand { Slug = slug, Uid = uid });
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

        [HttpPut("profile-picture/{slug}")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file,string slug, CancellationToken ct)
        {
            await using var stream = file.OpenReadStream();

            var path = await _mediator.Send(
                new UploadProfilePictureCommand
                {
                    Content = stream,
                    CommunitySlug = slug
                });

            return Ok(new { path });
        }

        [HttpGet("profile-picture/{slug}")]
        public async Task<IActionResult> GetProfilePicture(string slug , CancellationToken ct)
        { 
            var stream = await _mediator.Send(new GetProfilePictureQuery { CommunitySlug = slug}, ct);
            return stream is null ? NotFound() : File(stream, "image/jpeg");
        }
    }
}