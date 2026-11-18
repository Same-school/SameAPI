using SameApi.Model;

namespace SameApi.Dto.Community
{
    public class CommunityCriteria
    {
        public string? Slug { get; set; }
        public CommunityTypeEnum? CommunityType { get; set; }
        public PostingPolicyEnum? PostingPolicy { get; set; }
        public CommunityStatusEnum? CommunityStatus { get; set; }
    }
}