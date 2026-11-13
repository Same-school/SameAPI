using Google.Cloud.Firestore;
using SameApi.Data.Model;
namespace SameApi.Model
{
    [FirestoreData]
    public class CommunityDao : IModelDao
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "";
        
        [FirestoreProperty("name")]
        public string Name { get; set; } = "";
        [FirestoreProperty("nameLower")]
        public string NameLower { get; set; } = "";

        [FirestoreProperty("description")]
        public string Description { get; set; } = "";

        [FirestoreProperty("type", ConverterType = typeof(FirestoreEnumNameConverter<CommunityTypeEnum>))]
        public CommunityTypeEnum Type { get; set; }
        [FirestoreProperty("status", ConverterType = typeof(FirestoreEnumNameConverter<CommunityStatusEnum>))]
        public CommunityStatusEnum Status { get; set; } = CommunityStatusEnum.pending;

        [FirestoreProperty("postingPolicy", ConverterType = typeof(FirestoreEnumNameConverter<PostingPolicyEnum>))]
        public PostingPolicyEnum PostingPolicy { get; set; } = PostingPolicyEnum.MembersOnly;

        [FirestoreProperty("schoolId")]
        public string? SchoolId { get; set; }

        [FirestoreProperty("membersCount")]
        public int MembersCount { get; set; }

        [FirestoreProperty("postsCount")]
        public int PostsCount { get; set; }

        [FirestoreProperty("createdBy")]
        public string CreatedBy { get; set; } = "";

        [FirestoreProperty("reviewedBy")]
        public string? ReviewedBy { get; set; }

        [FirestoreProperty("reviewedAt")]
        public DateTime? ReviewedAt { get; set; }

        [FirestoreProperty("rejectionReason")]
        public string? RejectionReason { get; set; }

        [FirestoreProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [FirestoreProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}