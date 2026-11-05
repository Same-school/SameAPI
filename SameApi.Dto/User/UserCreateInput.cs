namespace SameApi.Dto.User
{
    public class UserCreateInput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? BirthDate { get; set; }
        public bool? IsPrivate { get; set; }
        public string? SchoolId { get; set; }
    }
}