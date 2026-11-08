using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository
{
    public interface IUserRepository : IBaseRepository<IApiSameDbContext, UserDao>
    {
        Task<UserDao?> GetByEmailAsync(string email);
        Task CreateAsync(UserDao user);                
        Task<bool> EmailExistsAsync(string? username);
        Task<List<string>> LoadFollowedIdsAsync(string userId);
        Task UpdateFieldsAsync(string id, Dictionary<string, object> updates);
        Task FollowAsync(string followerId, string followedId);
        Task UnfollowAsync(string followerId, string followedId);
        Task VoteAsync(string userId, string postId, int value);
    }
}