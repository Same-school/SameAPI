using AutoMapper;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Community;

namespace SameApi.Business.Community.Query
{
    public class GetCommunityByCriteriaQuery : CommunityCriteria, IRequest<List<CommunityResponse>>
    {
    }
    public class GetCommunityByCriteriaQueryHandler : IRequestHandler<GetCommunityByCriteriaQuery, List<CommunityResponse>>
    {
        readonly IApiSameUnitOfWork _uow;
        readonly IMapper _mapper;
        public GetCommunityByCriteriaQueryHandler(
            IApiSameUnitOfWork uow,
            IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<CommunityResponse>> Handle(GetCommunityByCriteriaQuery request, CancellationToken cancellationToken)
        {
            var communities = await _uow.CommunityRepository.GetByCriteriaAsync(request);
            return _mapper.Map<List<CommunityResponse>>(communities);
        }
    }
}