using MediatR;
using SameApi.Db.UnitOfWork;
using System.Text.Json.Serialization;

namespace SameApi.Business.Community.Command
{
    public class RejectCommunityCommand : IRequest
    {
        [JsonIgnore]
        public string Slug { get; set; } = "";
        public string Reason { get; set; } = "";

        [JsonIgnore]
        public string? Uid { get; set; }
    }
    public class RejectCommunityCommandHandler : IRequestHandler<RejectCommunityCommand>
    {
        private readonly IApiSameUnitOfWork _uow;
        public RejectCommunityCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task Handle(RejectCommunityCommand request, CancellationToken cancellationToken)
        {
            if (request.Uid is null)
                throw new UnauthorizedAccessException("User not logged");

            await _uow.CommunityRepository.RejectAsync(request.Slug, request.Uid, request.Reason);
        }
    }
}
