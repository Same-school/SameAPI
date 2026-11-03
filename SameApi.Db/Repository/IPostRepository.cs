using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository
{
    public interface IPostRepository : IBaseRepository<IApiSameDbContext, PostDao>
    {
        Task CreateAsync(PostDao post);
        Task<List<PostDao>> GetByUserIdAsync(string userId);
        Task<List<PostDao>> GetRecentPostsByAuthorsAsync(IReadOnlyList<string> authorIds, Timestamp since, int limitPerChunk);
    }
}