using SameApi.Db.DbContexts;
using SameApi.Db.Repository;

namespace SameApi.Db.UnitOfWork
{
    public interface IApiSameUnitOfWork
    {
        IApiSameDbContext Context { get; }
        IUserRepository UserRepository { get; }
        IPostRepository PostRepository { get; }
        ICommentRepository CommentRepository { get; }
        ICommunityRepository CommunityRepository { get; }
        //Task<int> SaveChangesAsync();

    }
}