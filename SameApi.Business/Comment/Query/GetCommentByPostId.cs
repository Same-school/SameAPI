using AutoMapper;
using Google.Cloud.Firestore;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Comment;

namespace SameApi.Business.Comment.Query
{
    public class GetCommentByPostId : IRequest<List<CommentResponse>?>
    {
        public required string Id { get; set; }
        public DateTime? After { get; set; }
    }

    public class GetCommentByPostIdHandler : IRequestHandler<GetCommentByPostId, List<CommentResponse>?>
    {
        private const int PageSize = 100;

        readonly IApiSameUnitOfWork _uow;
        readonly IMapper _mapper;

        public GetCommentByPostIdHandler(IApiSameUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<CommentResponse>?> Handle(GetCommentByPostId request, CancellationToken ct)
        {
            var result = await _uow.CommentRepository.GetByPostAsync(request.Id, PageSize);

            var items = _mapper.Map<List<CommentResponse>>(result);
            items = items.OrderByDescending(x => x.CreatedAt).ToList();
            return items;
        }
    }
}