using MediatR;
using SameApi.Db.UnitOfWork;
using System.Text.Json.Serialization;

namespace SameApi.Business.Community.Query
{
    public class ApproveCommunityQuery : IRequest
    {
        public string Slug { get; set; } = "";

        [JsonIgnore]
        public string? Uid { get; set; }
    }
    public class ApproveCommunityQueryHandler : IRequestHandler<ApproveCommunityQuery>
    {
        private readonly IApiSameUnitOfWork _uow;

        public ApproveCommunityQueryHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task Handle(ApproveCommunityQuery request, CancellationToken cancellationToken)
        {
            if (request.Uid is null)
                throw new UnauthorizedAccessException("User not logged");

            await _uow.CommunityRepository.ApproveAsync(request.Slug.ToLowerInvariant(), request.Uid);
        }
    }
}