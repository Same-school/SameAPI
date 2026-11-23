using AutoMapper;
using Google.Cloud.Firestore;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Post;

namespace SameApi.Business.Post.Query
{
    public class GetAllPostQuery : IRequest<List<PostResponse>>
    {
        public string userId;
    }
    public class GetAllPostQueryHandler : IRequestHandler<GetAllPostQuery, List<PostResponse>>
    {
        private const int DayLimit = 7;
        private const int PageSize = 20;
        private const int PoolPerChunk = 50;
        IApiSameUnitOfWork _apiSameUnitOfWork;
        IMapper _mapper;

        public GetAllPostQueryHandler(
            IApiSameUnitOfWork apiSameUnitOfWork,
            IMapper mapper)
        {
            _apiSameUnitOfWork = apiSameUnitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PostResponse>> Handle(GetAllPostQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var followedIds = await _apiSameUnitOfWork.UserRepository.LoadFollowedIdsAsync(request.userId);

            var since = Timestamp.FromDateTime(now.AddDays(-DayLimit));

            var candidates = followedIds.Count == 0 ? await _apiSameUnitOfWork.PostRepository.GetRecentPostsAsync(since, PoolPerChunk) 
                                                    : await _apiSameUnitOfWork.PostRepository.GetRecentPostsByAuthorsAsync(followedIds, since, PoolPerChunk);
            var page = candidates
                .Take(PageSize)
                .ToList();


            return _mapper.Map<List<PostResponse>>(page);
        }
    }
}