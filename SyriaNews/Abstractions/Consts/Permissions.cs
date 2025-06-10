namespace SyriaNews.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; set; } = "permission";

    //Newspaper
    public const string PostArticleNewspaper = "newspaper:post_article";
    public const string SetImagesNewspaper = "newspaper:set_images_for_article";
    public const string SetTagNewspaper = "newspaper:set_tags_for_article";
    public const string UpdateArticleNewspaper = "newspaper:update_article";
    public const string UpdateNewspaper = "newspaper:update";
    public const string ChangeProfileImageNewspaper = "newspaper:change_profile_image";
    public const string RemoveProfileImageNewspaper = "newspaper:remove_profile_image";
    public const string MyProfileNewspaper = "newspaper:me";
    public const string ChangePasswordNewspaper = "newspaper:change_password";

    //Member
    public const string CommentOnArticleMember = "member:comment";
    public const string LikeArticleMember = "member:like";
    public const string FollowingNewspaperMember = "member:follow";
    public const string UpdateCommentMember = "member:update_comment";
    public const string RemoveCommentMember = "member:remove_comment";
    public const string RemoveLikeMember = "member:remove_like";
    public const string RemoveFollowMember = "member:remove_follow";
    public const string SaveArticleMember = "member:save";
    public const string UnsaveArticleMember = "member:remove_save";
    public const string UpdateMemberProfileMember = "member:update";
    public const string MyProfileMember = "member:me";
    public const string ChangeProfileImageMember = "member:change_profile_image";
    public const string RemoveProfileImageMember = "member:remove_profile_image";
    public const string ChangePasswordMember = "member:change_password";


    //admin and newspaper
    public const string ToggleArticleNewspaperAdmin = "admin_newspaper:toggle_status_article";
    public const string CalculateAllViewsForCategoryNewspaperAdmin = "admin_newspaper:calculate_views_by_category";
    public const string CalculateAllLikesForCategoryNewspaperAdmin = "admin_newspaper:calculate_likes_by_category";
    public const string UsedCategoryNewspaperAdmin = "admin_newspaper:used_categories";
    public const string CountOfLikesNewspaperAdmin = "admin_newspaper:count_of_likes";
    public const string CountOfViewsNewspaperAdmin = "admin_newspaper:count_of_views";
    public const string UsedTagsNewspaperAdmin = "admin_newspaper:used_tags";
    public const string ArticlesOfNewspaperAdmin = "admin_newspaper:my_articles";
    public const string ReadArticleNewspaperAdminMemberVisitor = "admin_newspaper:read_article_as_admin";


    //admin and member and newspaper
    public const string CountOfPublishedArticlesNewspaperAdminMember = "admin_member_newspaper:count_of_published_articles";

    //admin and membre and newspaper and visitor
    public const string ShowMembersLikesNewspaperAdminMemberVisitor = "admin_member_newspaper_visitor:show_members_likes";
    public const string VisitMemberProfileNewspaperAdminMemberVisitor = "admin_member_newspaper_visitor:member_profile";
    public const string CommentsForArticleNewspaperAdminMemberVisitor = "admin_member_newspaper_visitor:comments_for_article";


    //admin and member
    public const string FullSavedArtcielsMemberAdmin = "admin_member:saved_artciels";
    public const string LikedArticleMemberAdmin = "admin_member:liked_artciels";
    public const string FollowedNewspapersMemberAdmin = "admin_member:followed_newspaper";
    public const string MemberCommentsMemberAdmin = "admin_member:my_comments";

    //admin and member and visitor
    public const string ShowNewArticlesByPostDateMemberAdminVisitor = "admin_member_visitor:show_new_articles_by_post_date";
    public const string ShowNewArticlesByMostViewedMemberAdminVisitor = "admin_member_visitor:show_new_articles_by_most_viewed";
    public const string ShowNewArticlesByTagMemberAdminVisitor = "admin_member_visitor:show_new_articles_by_tag";
    public const string ShowNewArticlesByCategoryMemberAdminVisitor = "admin_member_visitor :show_new_articles_by_category";
    public const string ShowNewArticlesByTitleMemberAdminVisitor = "admin_member_visitor:show_new_articles_by_title";
    public const string ReadArticleMemberAdminVisitor = "admin_member_visitor:read_as_member";
    public const string SearchForNewspaperMemberAdminVisitor = "admin_member_visitor:search_for_newspaper";
    public const string VisitNewspaperProfileMemberAdminVisitor = "admin_member_visitor:visit_newspaper_profile";
    public const string ArticleByNewspaperMemberAdminVisitor = "admin_member_visitor:article_by_newspaper";
    public const string CommentsForArticleMemberAdminVisitor = "admin_member_visitor:comments_for_article";
    public const string ShowMembersLikesMemberAdminVisitor = "admin_member_visitor:show_members_likes";
    public const string ShowMemberFollowingMemberAdminVisitor = "admin_member_visitor:show_member_following";

    //admin and newspaper and member and visitor 
    public const string VisitMemberProfileMemberAdminNewspaperVisitor = "admin_member_newspaper_visitor:visit_member_profile";


    //Admin
    public const string ToggleNewspaperStatusAdmin = "admin:toggle_newspaper_status";
    public const string ToggleMemberStatusAdmin = "admin:toggle_member_status";
    public const string ArticleToggleStatusAdmin = "admin:toggle_article_status";
    public const string ConfirmUserAccountAdmin = "admin:confirm_user_account";
    public const string SeeNewArticlesAdmin = "admin:see_new_articles";
    public const string SeeNewArticlesByCategoryAdmin = "admin:see_new_articles_by_category";
    public const string SeeNewArticlesByTagAdmin = "admin:see_new_articles_by_tag";
    public const string SeeNewArticlesTitleAdmin = "admin:see_new_articles_title";
    public const string SeeNewArticlesByViewsAdmin = "admin:see_new_articles_by_views";
    public const string SearchForNewspaperByNameAdmin = "admin:search_for_newspaper_by_name";
    public const string SeeNewspaperProfileAdmin = "admin:see_newspaper_profile";
    public const string SeeArticlesForNewspaperAdmin = "admin:see_articles_for_newspaper";
    public const string CommentsForArticleAdmin = "admin:comments_for_article";
    public const string ShowArticlesLikesAdmin = "admin:show_articles_likes";
    public const string MemberCommentsAdmin = "admin:member_comments";
    public const string MemeberFollowsAdmin = "admin:memeber_follows";
    public const string MemeberSavesAdmin = "admin:memeber_saves";
    public const string MemeberLikesAdmin = "admin:memeber_likes";
    public const string SeeMemberProfileAdmin = "admin:see_member_profile";
    public const string RemoveCommentAdmin = "admin:remove_comment";
    public const string ReadArticleAdmin = "admin:read_article";

}
