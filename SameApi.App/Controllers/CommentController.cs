using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameApi.Business.Comment.Command;
using SameApi.Business.Comment.Query;

namespace SameApi.App.Controllers
{
    [ApiController]
    [Route("api/comment")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentCommand command, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value;
            if (uid is null) return Unauthorized();

            command.Uid = uid;
            await _mediator.Send(command, ct);
            return Ok();
        }
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentByPostId(string postId, CancellationToken ct)
        {
            var comment = await _mediator.Send(new GetCommentByPostId { Id = postId}, ct);
            return comment is null ? NotFound() : Ok(comment);
        }
    }
}