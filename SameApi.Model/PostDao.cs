using Google.Cloud.Firestore;
using SameApi.Data.Model;
namespace SameApi.Model
{
    [FirestoreData]
    public class PostDao : IModelDao
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "";

        [FirestoreProperty("communityId")]
        public string? CommunityId { get; set; }

        [FirestoreProperty("authorId")]
        public string AuthorId { get; set; } = "";

        [FirestoreProperty("title")]
        public string Title { get; set; } = "";

        [FirestoreProperty("text")]
        public string? Text { get; set; }

        [FirestoreProperty("imageUrl")]
        public string? ImageUrl { get; set; }

        [FirestoreProperty("commentsCount")]
        public int CommentsCount { get; set; }

        [FirestoreProperty("createdAt")]
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        public Timestamp UpdatedAt { get; set; }

        [FirestoreProperty("isDeleted")]
        public bool IsDeleted { get; set; }


        // handle upvote and downvote .... 

        // like commentCount but for the feed order by the most upvoted
    }
}