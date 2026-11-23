using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Model.Enum;
using System.Text.Json.Serialization;

namespace SameApi.Business.Community.Command
{
    public class JoinCommunityCommand : IRequest
    {
        public string Slug { get; set; } = "";

        [JsonIgnore]
        public string? Uid { get; set; }
    }

    public class JoinCommunityCommandHandler : IRequestHandler<JoinCommunityCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public JoinCommunityCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task Handle(JoinCommunityCommand request, CancellationToken cancellationToken)
        {
            if (request.Uid is null)
                throw new UnauthorizedAccessException("User not logged");

            await _uow.CommunityRepository.JoinAsync(request.Slug.ToLowerInvariant(), request.Uid);
        }
    }
}