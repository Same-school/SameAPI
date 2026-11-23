using MediatR;
using SameApi.Db.UnitOfWork;

namespace SameApi.Business.Community.Command
{
    public class DeleteCommunityCommand : IRequest
    {
        public required string CommunityId { get; set; }
    }
    public class DeleteCommunityCommandHandler : IRequestHandler<DeleteCommunityCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public DeleteCommunityCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var community = await _uow.CommunityRepository.GetByIdAsync(request.CommunityId) ?? throw new NullReferenceException("Error when checking Community Id, This id doesn't exist");
            
            community.IsDeleted = true;
            community.UpdatedAt = now;

            await _uow.CommunityRepository.UpdateAsync(community);
        }
    }
}