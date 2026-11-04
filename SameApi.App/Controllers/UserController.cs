using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameApi.Business.User.Command;
using SameApi.Business.User.Query;

namespace SameApi.App.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var requesterId = User.FindFirst("user_id")?.Value;

            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery { Id = id, RequesterId = requesterId }, ct);
                return user is null ? NotFound() : Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            var user = await _mediator.Send(new GetUserByIdQuery { Id = uid, RequesterId = uid }, ct);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.Uid = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserCommand command)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.Id = uid;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPatch("follow")]
        public async Task<IActionResult> FollowAsync([FromBody] FollowUserCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.CurrentUserId = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpPatch("unfollow")]
        public async Task<IActionResult> UnFollowAsync([FromBody] UnfollowUserCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.CurrentUserId = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return Ok();
        }

        [HttpPut("profile-picture")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> GetProfilePicture(IFormFile file, CancellationToken ct)
        {
            var requesterId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            await using var stream = file.OpenReadStream();
            var path = await _mediator.Send(
                new UploadProfilePictureCommand
                {
                    Content = stream,
                    Uid = requesterId
                });

            return Ok(new { path });
        }

        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetMyProfilePicture(CancellationToken ct)
        {
            var requesterId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            var stream = await _mediator.Send(new GetProfilePictureQuery { Uid = requesterId }, ct);
            return stream is null ? NotFound() : File(stream, "image/jpeg");
        }

    }
}