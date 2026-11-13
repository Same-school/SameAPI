using AutoMapper;
using Google.Cloud.Firestore;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.User;
using SameApi.Model;
using SameApi.Model.Enum;
using System.Text.Json.Serialization;

namespace SameApi.Business.User.Command
{
    public class CreateUserCommand : UserCreateInput, IRequest
    {
        [JsonIgnore]
        public string? Uid { get; set; } = default!;
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IApiSameUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
            IApiSameUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _uow.UserRepository.EmailExistsAsync(request.Email))
                throw new InvalidOperationException("A user with this email already exists.");

            var now = Timestamp.GetCurrentTimestamp();

            UserDao user = _mapper.Map<UserDao>(request);

            if (request.Email != null )
                user.Username = request.Email.Split('@')[0];

            if (string.IsNullOrEmpty(user.DisplayName))
                user.DisplayName = user.Username;

            user.Id = request.Uid!;
            user.CreatedAt = now;
            user.UpdatedAt = now;
            user.Role = AppRoleEnum.Member;

            await _uow.UserRepository.CreateAsync(user);
        }
    }
}