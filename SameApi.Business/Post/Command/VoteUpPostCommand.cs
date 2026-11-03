using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Post;
using System.Text.Json.Serialization;

namespace SameApi.Business.Post.Command
{
    public class VoteUpPostCommand : IRequest
    {
        [JsonIgnore]
        public string? CurrentUserId { get; set; }
        public string? PostId { get; set; }
    }
    public class VoteUpPostCommandHandler : IRequestHandler<VoteUpPostCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public VoteUpPostCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task Handle(VoteUpPostCommand request, CancellationToken cancellationToken)
        {
            await _uow.UserRepository.VoteAsync(request.CurrentUserId, request.PostId, +1);
        }
    }
}