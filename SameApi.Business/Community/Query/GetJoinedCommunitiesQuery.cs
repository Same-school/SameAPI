using AutoMapper;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Community;

namespace SameApi.Business.Community.Query
{
    public class GetJoinedCommunitiesQuery : IRequest<List<CommunityResponse>>
    {
        public required string Uid { get; set; }
    }

    public class GetJoinedCommunitiesQueryHandler(IApiSameUnitOfWork uow, IMapper mapper)
        : IRequestHandler<GetJoinedCommunitiesQuery, List<CommunityResponse>>
    {
        public async Task<List<CommunityResponse>> Handle(GetJoinedCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communities = await uow.CommunityRepository.GetJoinedByUserAsync(request.Uid);
            return mapper.Map<List<CommunityResponse>>(communities);
        }
    }
}
