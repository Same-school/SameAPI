using AutoMapper;
using FirebaseAdmin.Auth;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using SameApi.Db.Repository.Implementation;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.User;
using SameApi.Model;
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
            //if (await _uow.UserRepository.EmailExistsAsync(request.Email))
            //    throw new InvalidOperationException("A user with this email already exists.");

            //var now = Timestamp.GetCurrentTimestamp();

            //UserDao user = _mapper.Map<UserDao>(request);

            //if (request.Email != null )
            //    user.Username = request.Email.Split('@')[0];

            //if (string.IsNullOrEmpty(user.DisplayName))
            //    user.DisplayName = user.Username;

            //user.Id = request.Uid!;
            //user.CreatedAt = now;
            //user.UpdatedAt = now;

            //await _uow.UserRepository.CreateAsync(user);

            var existing = await _uow.UserRepository.GetByEmailAsync(request.Email);

            if (existing != null && existing.Id != request.Uid)
            {
                if (await FirebaseUserExistsAsync(existing.Id, cancellationToken))
                    throw new EmailAlreadyUsedException("Email already used", StatusCodes.Status409Conflict);
                await _uow.UserRepository.RemoveByIdAsync(existing.Id);
            }
            var input = _mapper.Map<UserDao>(request);
            input.Id = request.Uid ?? throw new NullReferenceException();

            await _uow.UserRepository.CreateAsync(input);
        }
        private static async Task<bool> FirebaseUserExistsAsync(string uid, CancellationToken cancellationToken)
        {
            try
            {
                await FirebaseAuth.DefaultInstance.GetUserAsync(uid, cancellationToken);
                return true;
            }
            catch (FirebaseAuthException e) when (e.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return false;
            }

        }
    }

    public class EmailAlreadyUsedException(string message, int statusCode = 500) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }
}
