using AutoMapper;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.Community;
using SameApi.Model;
using System.Text.Json.Serialization;

namespace SameApi.Business.Community.Command
{
    public class CreateCommunityCommand : CommunityCreateInput, IRequest
    {
        [JsonIgnore]
        public string? Uid { get; set; } = default!;
    }
    public class CreateCommunityCommandHandler : IRequestHandler<CreateCommunityCommand>
    {
        private readonly IApiSameUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateCommunityCommandHandler(
            IApiSameUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
        {
            if (request.Uid is null)
                throw new NullReferenceException("User not logged");

            var now = DateTime.UtcNow;

            CommunityDao community = _mapper.Map<CommunityDao>(request);

            community.NameLower = community.Name.ToLowerInvariant().Replace(" ", "-");
            community.Id = $"{community.NameLower}_{community.SchoolId}_{community.Type}";
            community.UpdatedAt = now;
            community.CreatedAt = now;
            community.CreatedBy = request.Uid;
            community.Type = CommunityTypeEnum.community;

            await _uow.CommunityRepository.CreateAsync(community);
        }
    }
}