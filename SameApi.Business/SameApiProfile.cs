using AutoMapper;
using SameApi.Dto.Comment;
using SameApi.Dto.Post;
using SameApi.Dto.User;
using SameApi.Model;

namespace SameApi.Business
{
    public class SameApiProfile : Profile
    {
        public SameApiProfile()
        {
            //User
            CreateMap<UserInput, UserDao>();
            CreateMap<UserDao, UserResponse>()
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.ToDateTime())
                );

            CreateMap<PostInput, PostDao>();
            CreateMap<PostDao, PostResponse>()
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.ToDateTime())
                );

            CreateMap<CommentInput, CommentDao>();
            CreateMap<CommentDao, CommentResponse>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt.ToDateTime()))
                .ForMember(d => d.Text, o => o.MapFrom(s => s.IsDeleted ? "" : s.Text));


        }
    }
}