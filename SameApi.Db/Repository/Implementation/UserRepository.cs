using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository.Implementation
{
    public class UserRepository(IApiSameDbContext context)
        : BaseRepository<IApiSameDbContext, UserDao>(context), IUserRepository
    {
        protected override string CollectionName => "users";

        public async Task CreateAsync(UserDao user)
        {
            var document = Collection.Document(user.Id);
            await document.CreateAsync(user);
        }

        public async Task<List<string>> LoadFollowedIdsAsync(string userId)
        {
            var db = Collection.Database;

            var snapshot = await db.Collection("follows")
                .WhereEqualTo("followerId", userId)
                .Select("followedId")
                .GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.GetValue<string>("followedId"))
                .ToList();
        }
        public async Task<bool> EmailExistsAsync(string? email)
        {
            var snap = await Collection.WhereEqualTo("email", email).Limit(1).GetSnapshotAsync();
            return snap.Count > 0;
        }

        public async Task UpdateFieldsAsync(string id, Dictionary<string, object> updates)
        {
            var document = Collection.Document(id);
            await document.UpdateAsync(updates);
        }

        public async Task FollowAsync(string followerId, string followedId)
        {
            var db = Collection.Database;
            var followRef = db.Collection("follows").Document($"{followerId}_{followedId}");
            var followerRef = Collection.Document(followerId);
            var followedRef = Collection.Document(followedId);
            var now = Timestamp.GetCurrentTimestamp();

            await db.RunTransactionAsync(async tx =>
            {
                var snap = await tx.GetSnapshotAsync(followRef);
                if (snap.Exists)
                    throw new InvalidOperationException("ALREADY_FOLLOWING: already followed.");

                tx.Create(followRef, new Dictionary<string, object>
                {
                    ["followerId"] = followerId,
                    ["followedId"] = followedId,
                    ["createdAt"] = now
                });
                tx.Update(followerRef, new Dictionary<string, object>
                {
                    ["followingCount"] = FieldValue.Increment(1),
                    ["updatedAt"] = now
                });
                tx.Update(followedRef, new Dictionary<string, object>
                {
                    ["followersCount"] = FieldValue.Increment(1),
                    ["updatedAt"] = now
                });
            });
        }

        public async Task UnfollowAsync(string followerId, string followedId)
        {
            var db = Collection.Database;
            var followRef = db.Collection("follows").Document($"{followerId}_{followedId}");
            var followerRef = Collection.Document(followerId);
            var followedRef = Collection.Document(followedId);
            var now = Timestamp.GetCurrentTimestamp();

            await db.RunTransactionAsync(async tx =>
            {
                var snap = await tx.GetSnapshotAsync(followRef);
                if (snap.Exists)
                {
                    tx.Delete(followRef);

                    tx.Update(followerRef, new Dictionary<string, object>
                    {
                        ["followingCount"] = FieldValue.Increment(-1),
                        ["updatedAt"] = now
                    });

                    tx.Update(followedRef, new Dictionary<string, object>
                    {
                        ["followersCount"] = FieldValue.Increment(-1),
                        ["updatedAt"] = now
                    });
                }
                else
                    throw new InvalidOperationException("ALREADY_FOLLOWING: no follow find.");
            });
        }
    }
}