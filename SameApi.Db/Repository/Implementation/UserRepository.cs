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

        public async Task VoteAsync(string userId, string postId, int value)
        {
            if (value != 1 && value != -1)
                throw new ArgumentException("INVALID_VOTE: value must be 1 or -1.");

            var db = Collection.Database;
            var voteRef = db.Collection("votes").Document($"{userId}_{postId}");
            var postRef = db.Collection("posts").Document(postId);
            var now = Timestamp.GetCurrentTimestamp();

            await db.RunTransactionAsync(async tx =>
            {
                var postSnap = await tx.GetSnapshotAsync(postRef);
                if (!postSnap.Exists || postSnap.GetValue<bool>("isDeleted"))
                    throw new KeyNotFoundException("POST_NOT_FOUND: post not found.");

                var voteSnap = await tx.GetSnapshotAsync(voteRef);

                long up = postSnap.GetValue<long>("upvotesCount");
                long down = postSnap.GetValue<long>("downvotesCount");

                if (voteSnap.Exists)
                {
                    int previous = voteSnap.GetValue<int>("value");
                    if (previous == value) 
                        return;

                    if (previous == 1) 
                        up--; 
                    else 
                        down--;
                    tx.Update(voteRef, new Dictionary<string, object>
                    {
                        ["value"] = value,
                        ["updatedAt"] = now
                    });
                }
                else
                {
                    tx.Create(voteRef, new Dictionary<string, object>
                    {
                        ["userId"] = userId,
                        ["targetId"] = postId,
                        ["targetType"] = "post",
                        ["value"] = value,
                        ["createdAt"] = now,
                        ["updatedAt"] = now
                    });
                }

                if (value == 1) 
                    up++; 
                else 
                    down++;

                tx.Update(postRef, new Dictionary<string, object>
                {
                    ["upvotesCount"] = up,
                    ["downvotesCount"] = down,
                    ["wilsonScore"] = Scoring.WilsonLowerBound(up, down)
                });
            });
        }
    }
}