using SameApi.Model;

namespace SameApi.Dto.Community
{
    public class CommunityCreateInput
    {
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public string? SchoolId { get; set; }
        public CommunityTypeEnum CommunityType { get; set; }
    }
}