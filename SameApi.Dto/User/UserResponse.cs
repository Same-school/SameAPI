namespace SameApi.Dto.User
{
    public class UserResponse
    {
        public string Id { get; set; } = "";
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPrivate { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? SchoolId { get; set; }
    }
}