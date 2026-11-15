using MediatR;
using SameApi.Db.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SameApi.Business.Community.Command
{
    public class LeaveCommunityCommand : IRequest
    {
        public string Slug { get; set; } = "";

        [JsonIgnore]
        public string? Uid { get; set; }
    }
    public class LeaveCommunityCommandHandler : IRequestHandler<LeaveCommunityCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public LeaveCommunityCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task Handle(LeaveCommunityCommand request, CancellationToken cancellationToken)
        {
            if (request.Uid is null)
                throw new UnauthorizedAccessException("User not logged");

            await _uow.CommunityRepository.LeaveAsync(request.Slug.ToLowerInvariant(), request.Uid);
        }
    }
}