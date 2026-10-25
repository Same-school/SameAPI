using AutoMapper;
using FluentValidation.Internal;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Post;
using SameApi.Dto.User;

namespace SameApi.Business.Post.Query
{
    public class GetAllPostQuery : IRequest<List<PostResponse>>
    {
    }
    public class GetAllPostQueryHandler : IRequestHandler<GetAllPostQuery,List<PostResponse>>
    {
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
            var post = await _apiSameUnitOfWork.PostRepository.GetAllAsync();

            var response = post.Where(x => x.IsDeleted == false).ToList();

            return _mapper.Map<List<PostResponse>>(response);
        }
    }
}