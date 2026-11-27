using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Dto.Community;
using SameApi.Model;
using SameApi.Model.Enum;

namespace SameApi.Db.Repository.Implementation
{
    public class CommunityRepository(IApiSameDbContext context) : BaseRepository<IApiSameDbContext, CommunityDao>(context), ICommunityRepository
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
                    tx.Update(communityRef, "membersCount", FieldValue.Increment(1));
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
                if (community.IsDeleted || community.Status != CommunityStatusEnum.Approved)
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
        public async Task LeaveAsync(string slug, string uid)
        {
            var db = Collection.Database;
            var communityRef = Collection.Document(slug);
            var membershipRef = db.Collection("membership").Document($"{uid}_{slug}");
            var now = Timestamp.GetCurrentTimestamp();

            await db.RunTransactionAsync(async tx =>
            {
                var snap = await tx.GetSnapshotAsync(membershipRef);
                if (snap.Exists)
                {
                    tx.Delete(membershipRef);

                    tx.Update(communityRef, new Dictionary<string, object>
                    {
                        ["membersCount"] = FieldValue.Increment(-1),
                        ["updatedAt"] = now
                    });
                }
                else
                    throw new InvalidOperationException($"Membership not found for user '{uid}' in community '{slug}': User is not a member of this community.");
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

                if (community.Status != CommunityStatusEnum.Pending)
                    throw new InvalidOperationException("Community is not pending");

                var now = DateTime.UtcNow;
                tx.Update(communityRef, new Dictionary<string, object>
                {
                    ["status"] = CommunityStatusEnum.Approved.ToString(),
                    ["reviewedBy"] = adminUid,
                    ["reviewedAt"] = now,
                    ["updatedAt"] = now
                });
            });
        }
        
        public async Task<List<CommunityDao>> GetByCriteriaAsync(CommunityCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Slug))
            {
                var snap = await Collection.Document(criteria.Slug).GetSnapshotAsync();
                if (!snap.Exists)
                    return [];

                var community = snap.ConvertTo<CommunityDao>();
                return Matches(community, criteria) ? [community] : [];
            }

            Query query = Collection.WhereEqualTo("isDeleted", false);

            if (criteria.CommunityType.HasValue)
                query = query.WhereEqualTo("type", criteria.CommunityType.Value.ToString());

            if (criteria.PostingPolicy.HasValue)
                query = query.WhereEqualTo("postingPolicy", criteria.PostingPolicy.Value.ToString());

            if (criteria.CommunityStatus.HasValue)
                query = query.WhereEqualTo("status", criteria.CommunityStatus.Value.ToString());

            var result = await query.GetSnapshotAsync();
            return result.Documents.Select(d => d.ConvertTo<CommunityDao>()).ToList();
        }

        private static bool Matches(CommunityDao c, CommunityCriteria criteria) =>
            !c.IsDeleted
            && (!criteria.CommunityType.HasValue || c.Type == criteria.CommunityType)
            && (!criteria.PostingPolicy.HasValue || c.PostingPolicy == criteria.PostingPolicy)
            && (!criteria.CommunityStatus.HasValue || c.Status == criteria.CommunityStatus);

        public async Task<List<CommunityDao>> GetJoinedByUserAsync(string uid)
        {
            var db = Collection.Database;

            var memberships = await db.Collection("membership")
                .WhereEqualTo("userId", uid)
                .WhereEqualTo("status", MembershipStatusEnum.Active)
                .GetSnapshotAsync();

            if (memberships.Count == 0) return [];

            var refs = memberships.Documents
                .Select(d => Collection.Document(d.GetValue<string>("communityId")))
                .ToList();

            var snaps = await db.GetAllSnapshotsAsync(refs);

            return snaps
                .Where(s => s.Exists)
                .Select(s => s.ConvertTo<CommunityDao>())
                .Where(c => !c.IsDeleted)
                .ToList();
        }

        public async Task RejectAsync(string slug, string adminUid, string reason)
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

                if (community.Status != CommunityStatusEnum.Pending)
                    throw new InvalidOperationException("Community is not pending");

                var now = DateTime.UtcNow;
                tx.Update(communityRef, new Dictionary<string, object>
                {
                    ["status"] = CommunityStatusEnum.Rejected.ToString(),
                    ["reviewedBy"] = adminUid,
                    ["reviewedAt"] = now,
                    ["updatedAt"] = now,
                    ["rejectionReason"] = reason
                });
            });

        }
    }
}