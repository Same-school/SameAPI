using SameApi.Model;

namespace SameApi.Dto.Community
{
    public class CommunityResponse
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public CommunityTypeEnum CommunityType { get; set; }
        public PostingPolicyEnum PostingPolicy { get; set; }
        public CommunityStatusEnum CommunityStatus { get; set; }
        public int MemberCount { get; set; }
    }
}