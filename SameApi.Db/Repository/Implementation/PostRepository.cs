using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository.Implementation
{
    public class PostRepository(IApiSameDbContext context) : BaseRepository<IApiSameDbContext, PostDao>(context), IPostRepository
    {
        protected override string CollectionName => "posts";

        public async Task CreateAsync(PostDao post)
        {
            var db = Collection.Database;
            var postRef = Collection.Document();
            var userRef = db.Collection("users").Document(post.AuthorId);
            var now = Timestamp.GetCurrentTimestamp();

            post.CreatedAt = now;
            post.UpdatedAt = now;
            post.IsDeleted = false;

            await db.RunTransactionAsync(tx =>
            {
                tx.Create(postRef, post);
                tx.Update(userRef, new Dictionary<string, object>
                {
                    ["postsCount"] = FieldValue.Increment(1),
                    ["updatedAt"] = now
                });
                return Task.CompletedTask;
            });
            post.Id = postRef.Id;
        }
        public virtual async Task<List<PostDao>> GetByUserIdAsync(string userId)
        {
            var snap = await Collection.WhereEqualTo("authorId", userId).GetSnapshotAsync();
            return snap.Documents.Select(d => d.ConvertTo<PostDao>()).ToList();
        }
        public async Task<List<PostDao>> GetRecentPostsByAuthorsAsync(IReadOnlyList<string> authorIds, Timestamp since, int limitPerChunk)
        {
            if (authorIds.Count == 0) return new();

            var db = Collection.Database;

            var tasks = authorIds.Chunk(30).Select(chunk =>
                db.Collection("posts")
                    .WhereIn("authorId", chunk)
                    .WhereEqualTo("isDeleted", false)
                    .WhereGreaterThanOrEqualTo("createdAt", since)
                    .OrderByDescending("createdAt")
                    .Limit(limitPerChunk)
                    .GetSnapshotAsync());

            var snapshots = await Task.WhenAll(tasks);

            return snapshots
                .SelectMany(s => s.Documents)
                .Select(d => d.ConvertTo<PostDao>())
                .ToList();
        }
        public async Task<List<PostDao>> GetRecentPostsAsync(Timestamp since, int limit)
        {
            var snapshot = await Collection.Database.Collection("posts")
                .WhereEqualTo("isDeleted", false)
                .WhereGreaterThanOrEqualTo("createdAt", since)
                .OrderByDescending("createdAt")
                .Limit(limit)
                .GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<PostDao>()).ToList();
        }

    }
}