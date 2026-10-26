using Google.Cloud.Firestore;
using SameApi.Data.Repository;
using SameApi.Db.DbContexts;
using SameApi.Model;

namespace SameApi.Db.Repository.Implementation
{
    public class CommentRepository(IApiSameDbContext context)
            : BaseRepository<IApiSameDbContext, CommentDao>(context), ICommentRepository
    {
        protected override string CollectionName => "comments";

        public async Task CreateAsync(CommentDao comment)
        {
            var db = Collection.Database;
            var commentRef = Collection.Document();
            var postRef = db.Collection("posts").Document(comment.PostId);
            var now = Timestamp.GetCurrentTimestamp();

            comment.CreatedAt = now;
            comment.IsDeleted = false;

            await db.RunTransactionAsync(tx =>
            {
                tx.Create(commentRef, comment);
                tx.Update(postRef, new Dictionary<string, object>
                {
                    ["commentsCount"] = FieldValue.Increment(1),
                    ["updatedAt"] = now
                });
                return Task.CompletedTask;
            });

            comment.Id = commentRef.Id;
        }

        public async Task<IReadOnlyList<CommentDao>> GetByPostAsync(
            string postId, int limit = 50)
        {
            var query = Collection
                .WhereEqualTo("postId", postId);

            var snap = await query.Limit(limit).GetSnapshotAsync();
            return snap.Documents.Select(d => d.ConvertTo<CommentDao>()).ToList();
        }
    }
}