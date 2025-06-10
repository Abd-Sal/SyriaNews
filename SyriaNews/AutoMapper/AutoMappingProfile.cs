namespace SyriaNews.AutoMapper;

public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        //category
        MappingCategory();
        MappingCategoryResponse();

        //article
        MappingArticle();
        MappingArticleResponse();

        //tag
        MappingTag();
        MappingTagResponse();

        //save
        MappingSave();
        MappingSaveResponse();

        //like
        MappingLike();
        MappingLikeResponse();

        //follower
        MappingFollower();
        MappingFollowerResponse();

        //comment
        MappingComment();
        MappingCommentResponse();

        //articleTag
        MappingArticleTag();
        MappingArticleTagResponse();


        //articleImage
        MappingImage();
        MappingImageResponse();

        //profileImage
        MappingProfileImage();
        MappingProfileImageResponse();

        //member
        MappingMember();

        //role
        MappingRole();
    }

    //Role
    private void MappingRole()
        => CreateMap<ApplicationRole, RoleResponse>().ReverseMap();

    //Member
    private void MappingMember()
        => CreateMap<Member, MemberBreifResponse>()
            .ForMember(dest => dest.Gender, option => option.MapFrom(src => src.Gender ? "female" : "male"))
            .ReverseMap()
            .ForMember(dest => dest.Gender, option => option.MapFrom(src => src.Gender.ToLower() == "female"));


    //category
    private void MappingCategory()
        => CreateMap<Category, CategoryRequest>().ReverseMap();
    private void MappingCategoryResponse()
        => CreateMap<Category, CategoryResponse>().ReverseMap();


    //categories
    private void MappingArticle()
        => CreateMap<Article, ArticleRequest>().ReverseMap();
    private void MappingArticleResponse()
        => CreateMap<Article, ArticleBreifResponse>().ReverseMap();

    //tag
    private void MappingTag()
        => CreateMap<Tag, TagRequest>().ReverseMap();
    private void MappingTagResponse()
        => CreateMap<Tag, TagResponse>().ReverseMap();

    //save
    private void MappingSave()
        => CreateMap<Save, SaveRequest>().ReverseMap();
    private void MappingSaveResponse()
        => CreateMap<Save, SaveResponse>().ReverseMap();

    //like
    private void MappingLike()
        => CreateMap<Like, LikeRequest>().ReverseMap();
    private void MappingLikeResponse()
        => CreateMap<Like, LikeResponse>().ReverseMap();

    //follower
    private void MappingFollower()
        => CreateMap<Follower, FollowerRequest>().ReverseMap();
    private void MappingFollowerResponse()
        => CreateMap<Follower, FollowerResponse>().ReverseMap();

    //comment
    private void MappingComment()
        => CreateMap<Comment, CommentRequest>().ReverseMap();
    private void MappingCommentResponse()
        => CreateMap<Comment, CommentResponse>().ReverseMap();

    //articleTag
    private void MappingArticleTag()
        => CreateMap<ArticlesTags, ArticleTagRequest>().ReverseMap();
    private void MappingArticleTagResponse()
        => CreateMap<ArticlesTags, ArticleTagResponse>().ReverseMap();

    //articleImage
    private void MappingImage()
        => CreateMap<Image, ImageRequest>().ReverseMap();
    private void MappingImageResponse()
        => CreateMap<Image, ImageResponse>().ReverseMap();

    //profileImage
    private void MappingProfileImage()
        => CreateMap<ProfileImage, ProfileImageRequest>().ReverseMap();
    private void MappingProfileImageResponse()
        => CreateMap<ProfileImage, ProfileImageResponse>().ReverseMap();

}
