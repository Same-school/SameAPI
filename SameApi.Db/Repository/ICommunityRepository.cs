using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Dto.Community;
using SameApi.Model;

namespace SameApi.Db.Repository
{
    public interface ICommunityRepository : IBaseRepository<IApiSameDbContext, CommunityDao>
    {
        Task CreateAsync(CommunityDao comment);
        Task JoinAsync(string slug, string uid);
        Task LeaveAsync(string slug, string uid);
        Task ApproveAsync(string slug, string uid);
        Task RejectAsync(string slug, string uid, string reason);
        Task<List<CommunityDao>> GetJoinedByUserAsync(string uid);
        Task<List<CommunityDao>> GetByCriteriaAsync(CommunityCriteria criteria);
    }
}