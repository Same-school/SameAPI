using System.Reflection;
using System.Text.Json.Serialization;
using Google.Cloud.Firestore;
using MediatR;
using SameApi.Db.UnitOfWork;
using SameApi.Dto.User;

namespace SameApi.Business.User.Command
{
    public class UpdateUserCommand : UserUpdateInput, IRequest
    {
        [JsonIgnore]
        public string Id { get; set; } = "";
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IApiSameUnitOfWork _uow;

        public UpdateUserCommandHandler(IApiSameUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new ArgumentException("L'ID utilisateur est obligatoire.");

            var existingUser = await _uow.UserRepository.GetByIdAsync(request.Id) ?? throw new InvalidOperationException("USER_NOT_FOUND: User not found.");

            var updates = GetUpdatedFields(request);

            updates["updatedAt"] = Timestamp.GetCurrentTimestamp();

            await _uow.UserRepository.UpdateFieldsAsync(request.Id, updates);
        }

        public static Dictionary<string, object> GetUpdatedFields<T>(T model)
        {
            return typeof(T)
                .GetProperties()
                .Where(p => p.GetValue(model) != null)
                .Select(p => new {Property = p, Value = p.GetValue(model)})
                .Where(x => x.Property.Name != "Id")
                .ToDictionary(
                    x => x.Property.GetCustomAttribute<FirestorePropertyAttribute>()?.Name
                         ?? char.ToLowerInvariant(x.Property.Name[0]) + x.Property.Name[1..],
                    x => x.Value!
                );
        }
    }
}