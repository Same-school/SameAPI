using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameApi.Business.Post.Command;
using SameApi.Business.Post.Query;

namespace SameApi.App.Controllers
{
    [ApiController]
    [Route("api/post")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.Uid = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed()
        {
            var post = await _mediator.Send(new GetAllPostQuery());
            return post is null ? NotFound() : Ok(post);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var post = await _mediator.Send(new GetPostByIdQuery { Id = id }, ct);
            return post is null ? NotFound() : Ok(post);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken ct)
        {
            var post = await _mediator.Send(new GetPostByUserIdQuery { UserId = userId }, ct);
            return post is null ? NotFound() : Ok(post);
        }

        [HttpPatch("delete")]
        public async Task<IActionResult> DeletePost([FromBody] DeletePostCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpPatch("upvote")]
        public async Task<IActionResult> UpVotePost([FromBody] VoteUpPostCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) 
                return Unauthorized();

            command.CurrentUserId = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }

        [HttpPatch("downvote")]
        public async Task<IActionResult> DownVotePost([FromBody] VoteDownPostCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null)
                return Unauthorized();

            command.CurrentUserId = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }
    }
}