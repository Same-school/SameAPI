using AutoMapper;
using FirebaseAdmin.Auth;
using MediatR;
using SameApi.Db.UnitOfWork;

namespace SameApi.Business.User.Command
{
    public class DeleteUserCommand : IRequest<string>
    {
        public string Id { get; set; }
    }
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly IApiSameUnitOfWork _iuow;

        public DeleteUserCommandHandler(IMapper mapper, IApiSameUnitOfWork iuow)
        {
            _mapper = mapper;
            _iuow = iuow;
        }

        public async Task<string> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            await _iuow.UserRepository.RemoveByIdAsync(command.Id);
            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(command.Id, cancellationToken);
            }
            catch (FirebaseAuthException e) when (e.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
            }

            return command.Id;
        }
    }
}