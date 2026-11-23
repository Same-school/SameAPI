using Google.Cloud.Firestore;
using MediatR;
using SameApi.Db.UnitOfWork;

namespace SameApi.Business.Post.Command
{
    public class DeletePostCommand : IRequest
    {
        public required string PostId { get; set; }
    }
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public DeletePostCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _uow.PostRepository.GetByIdAsync(request.PostId) ?? throw new NullReferenceException("Error when checking PostId, This id doesn't exist");

            post.IsDeleted = true;
            post.UpdatedAt = Timestamp.GetCurrentTimestamp();

            await _uow.PostRepository.UpdateAsync(post);
        }
    }
}
