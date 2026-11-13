using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;
using SameApi.Model.Enum;

namespace SameApi.Db.Repository.Implementation
{
    public class CommunityRepository(IApiSameDbContext context)
        : BaseRepository<IApiSameDbContext, CommunityDao>(context), ICommunityRepository
    {
        protected override string CollectionName => "community";

        public async Task CreateAsync(CommunityDao community)
        {
            var db = Collection.Database;
            var communityRef = Collection.Document(community.Id);
            var membershipRef = db.Collection("membership").Document($"{community.CreatedBy}_{community.Id}");

            var ownerMembership = new MembershipDao
            {
                UserId = community.CreatedBy,
                CommunityId = community.Id,
                Role = MembershipRoleEnum.Owner,
                Status = MembershipStatusEnum.Active,
                JoinedAt = community.CreatedAt
            };

            try
            {
                await db.RunTransactionAsync(tx =>
                {
                    tx.Create(communityRef, community);
                    tx.Create(membershipRef, ownerMembership);
                    return Task.CompletedTask;
                });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
            {
                throw new InvalidOperationException("A community with this name already exists");
            }
        }

        public async Task JoinAsync(string slug, string uid)
        {
            var db = Collection.Database;
            var communityRef = Collection.Document(slug);
            var membershipRef = db.Collection("membership").Document($"{uid}_{slug}");

            await db.RunTransactionAsync(async tx =>
            {
                var communitySnap = await tx.GetSnapshotAsync(communityRef);
                var membershipSnap = await tx.GetSnapshotAsync(membershipRef);

                if (!communitySnap.Exists)
                    throw new KeyNotFoundException("Community not found");

                var community = communitySnap.ConvertTo<CommunityDao>();
                if (community.IsDeleted || community.Status != CommunityStatusEnum.approved)
                    throw new KeyNotFoundException("Community not found");

                if (membershipSnap.Exists)
                {
                    var existing = membershipSnap.ConvertTo<MembershipDao>();
                    if (existing.Status == MembershipStatusEnum.Banned)
                        throw new UnauthorizedAccessException("You are banned from this community");

                    throw new InvalidOperationException("Already a member");
                }

                var membership = new MembershipDao
                {
                    UserId = uid,
                    CommunityId = slug,
                    Role = MembershipRoleEnum.Member,
                    Status = MembershipStatusEnum.Active,
                    JoinedAt = DateTime.UtcNow
                };

                tx.Create(membershipRef, membership);
                tx.Update(communityRef, "membersCount", FieldValue.Increment(1));
            });
        }
        public async Task ApproveAsync(string slug, string adminUid)
        {
            var db = Collection.Database;
            var communityRef = Collection.Document(slug);

            await db.RunTransactionAsync(async tx =>
            {
                var snap = await tx.GetSnapshotAsync(communityRef);
                if (!snap.Exists)
                    throw new KeyNotFoundException("Community not found");

                var community = snap.ConvertTo<CommunityDao>();

                if (community.IsDeleted)
                    throw new KeyNotFoundException("Community not found");

                if (community.Status != CommunityStatusEnum.pending)
                    throw new InvalidOperationException("Community is not pending");

                var now = DateTime.UtcNow;
                tx.Update(communityRef, new Dictionary<string, object>
                {
                    ["status"] = CommunityStatusEnum.approved.ToString(),
                    ["reviewedBy"] = adminUid,
                    ["reviewedAt"] = now,
                    ["updatedAt"] = now
                });
            });
        }
    }
}