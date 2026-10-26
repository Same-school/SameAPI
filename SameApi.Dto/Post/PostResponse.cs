namespace SameApi.Dto.Post
{
    public class PostResponse
    {
        public string Id { get; set; } = "";
        public string? AuthorId { get; set; }
        public string? CommunityId { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public int CommentsCount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}