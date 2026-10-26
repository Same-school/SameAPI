namespace SameApi.Dto.Comment
{
    public class CommentResponse
    {
        public string Id { get; set; } = "";
        public string PostId { get; set; } = "";
        public string? ParentCommentId { get; set; }
        public string AuthorId { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}