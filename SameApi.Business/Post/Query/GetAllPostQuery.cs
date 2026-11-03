using AutoMapper;
using FluentValidation.Internal;
using Google.Cloud.Firestore;
using MediatR;
using Microsoft.VisualBasic;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Post;
using SameApi.Dto.User;
using SameApi.Model;

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
            if (followedIds.Count == 0)
                return new List<PostResponse>();

            var since = Timestamp.FromDateTime(now.AddDays(-DayLimit));

            var candidates = await _apiSameUnitOfWork.PostRepository
                .GetRecentPostsByAuthorsAsync(followedIds, since, PoolPerChunk);

            var page = candidates
                .Select(p => new
                {
                    Post = p,
                    Score = (p.WilsonScore + Scoring.BaseScore) * Scoring.Decay(p.CreatedAt.ToDateTime(), now)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Post.Id)
                .Take(PageSize)
                .Select(x => x.Post) 
                .ToList();
            return _mapper.Map<List<PostResponse>>(page);
        }
    }
}