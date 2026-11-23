using Google.Cloud.Firestore;
using SameApi.Data.Model;
using SameApi.Model.Enum;

namespace SameApi.Model
{
    [FirestoreData]
    public class UserDao : IModelDao
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "";

        [FirestoreProperty("userName")]
        public string Username { get; set; } = "";

        [FirestoreProperty("displayName")]
        public string DisplayName { get; set; } = "";

        [FirestoreProperty("firstName")]
        public string FirstName { get; set; } = "";

        [FirestoreProperty("lastName")]
        public string LastName { get; set; } = "";

        [FirestoreProperty("email")]
        public string Email { get; set; } = "";

        [FirestoreProperty("bio")]
        public string? Bio { get; set; }

        [FirestoreProperty("birthDate")]
        public string? BirthDate { get; set; }       

        [FirestoreProperty("isAdmin")]
        public bool IsAdmin { get; set; }

        [FirestoreProperty("isVerified")]
        public bool IsVerified { get; set; }

        [FirestoreProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [FirestoreProperty("followersCount")]
        public int FollowersCount { get; set; }

        [FirestoreProperty("followingCount")]
        public int FollowingCount { get; set; }

        [FirestoreProperty("postsCount")]
        public int PostsCount { get; set; }

        [FirestoreProperty("createdAt")]
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        public Timestamp UpdatedAt { get; set; }

        [FirestoreProperty("lastLoginAt")]
        public Timestamp? LastLoginAt { get; set; }

        [FirestoreProperty("schoolId")]
        public string? SchoolId { get; set; }

        [FirestoreProperty("role")]
        public AppRoleEnum Role { get; set; }
    }
}