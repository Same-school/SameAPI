using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository
{
    public interface ICommunityRepository : IBaseRepository<IApiSameDbContext, CommunityDao>
    {
        Task CreateAsync(CommunityDao comment);
        Task JoinAsync(string slug, string uid);
        Task ApproveAsync(string slug, string uid);
    }
}