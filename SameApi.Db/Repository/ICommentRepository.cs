using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository
{
    public interface ICommentRepository : IBaseRepository<IApiSameDbContext, CommentDao>
    {
        Task CreateAsync(CommentDao comment);
        Task<IReadOnlyList<CommentDao>> GetByPostAsync(string postId, int limit = 50);
    }
}