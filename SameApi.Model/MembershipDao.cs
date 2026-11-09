using Google.Cloud.Firestore;
using SameApi.Model.Enum;

namespace SameApi.Model
{
    [FirestoreData]
    public class MembershipDao
    {
        [FirestoreDocumentId] 
        public string Id { get; set; } = default!; 

        [FirestoreProperty("userId")]
        public string UserId { get; set; } = default!;

        [FirestoreProperty("communityId")]
        public string CommunityId { get; set; } = default!;

        [FirestoreProperty("role")] 
        public MembershipRoleEnum Role { get; set; }

        [FirestoreProperty("status")] 
        public MembershipStatusEnum Status { get; set; }

        [FirestoreProperty("joinedAt")] 
        public DateTime JoinedAt { get; set; }

        [FirestoreProperty("bannedBy")] 
        public string? BannedBy { get; set; }

        [FirestoreProperty("bannedAt")] 
        public DateTime? BannedAt { get; set; }

        [FirestoreProperty("banReason")] 
        public string? BanReason { get; set; }
    }
}