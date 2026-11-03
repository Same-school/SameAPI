using MediatR;
using SameApi.Db.UnitOfWork;
using System.Text.Json.Serialization;

namespace SameApi.Business.Post.Command
{
    public class VoteDownPostCommand : IRequest
    {
        [JsonIgnore]
        public string? CurrentUserId { get; set; }
        public string? PostId { get; set; }
    }

    public class VoteDownPostCommandHandler : IRequestHandler<VoteDownPostCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public VoteDownPostCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task Handle(VoteDownPostCommand request, CancellationToken cancellationToken)
        {
            await _uow.UserRepository.VoteAsync(request.CurrentUserId, request.PostId, -1);
        }
    }
}