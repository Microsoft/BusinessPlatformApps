using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookUtillity
{
    public static class FacebookPageAnalyticsMetricGroups
    {
        public const string PagePostStoriesAndPeopleTalkingAboutThis = "pagePostStoriesAndPeopleTalkingAboutThis";
        public const string PageImpressions = "pageImpressions";
        public const string PageEngagement = "pageEngagement";
        public const string PageReactions = "pageReactions";
        public const string PageCtaClicks = "pageCtaClicks";
        public const string PageUserDemographics = "pageUserDemographics";
        public const string PageContent = "pageContent";
        public const string PageViews = "pageViews";
        public const string PageVideoViews = "pageVideoViews";
        public const string PagePosts = "pagePosts";
        public const string PagePostImpressions = "pagePostImpressions";
        public const string PagePostEngagement = "pagePostEngagement";
        public const string PagePostReactions = "pagePostReactions";
        public const string PageVideoPosts = "pageVideoPosts";
        public const string PagePostIds = "pagePostIds";

        public const string PagePostStoriesAndPeopleTalkingAboutThisMetrics = "page_content_activity_by_action_type_unique,page_content_activity_by_age_gender_unique,page_content_activity_by_city_unique,page_content_activity_by_country_unique,page_content_activity_by_locale_unique,page_content_activity,page_content_activity_by_action_type,post_activity,post_activity_unique,post_activity_by_action_type,post_activity_by_action_type_unique,";
        public const string PageImpressionsMetrics = "page_impressions,page_impressions_unique,page_impressions_paid,page_impressions_paid_unique,page_impressions_organic,page_impressions_organic_unique,page_impressions_viral,page_impressions_viral_unique,page_impressions_by_story_type,page_impressions_by_story_type_unique,page_impressions_by_city_unique,page_impressions_by_country_unique,page_impressions_by_locale_unique,page_impressions_by_age_gender_unique,page_impressions_frequency_distribution,page_impressions_viral_frequency_distribution";
        public const string PageEngagementMetrics = "page_engaged_users,page_post_engagements,page_consumptions,page_consumptions_unique,page_consumptions_by_consumption_type,page_consumptions_by_consumption_type_unique,page_places_checkin_total,page_places_checkin_total_unique,page_places_checkin_mobile,page_places_checkin_mobile_unique,page_places_checkins_by_age_gender,page_places_checkins_by_locale,page_places_checkins_by_country,page_negative_feedback,page_negative_feedback_unique,page_negative_feedback_by_type,page_negative_feedback_by_type_unique,page_positive_feedback_by_type,page_positive_feedback_by_type_unique,page_fans_online,page_fans_online_per_day,page_fan_adds_by_paid_non_paid_unique";
        public const string PageReactionsMetrics = "page_actions_post_reactions_total";
        public const string PageCtaClicksMetrics = "page_total_actions,page_cta_clicks_logged_in_total,page_cta_clicks_logged_in_unique,page_cta_clicks_by_site_logged_in_unique,page_cta_clicks_by_age_gender_logged_in_unique,page_cta_clicks_logged_in_by_country_unique,page_cta_clicks_logged_in_by_city_unique,page_call_phone_clicks_logged_in_unique,page_call_phone_clicks_by_age_gender_logged_in_unique,page_call_phone_clicks_logged_in_by_country_unique,page_call_phone_clicks_logged_in_by_city_unique,page_call_phone_clicks_by_site_logged_in_unique,page_get_directions_clicks_logged_in_unique,page_get_directions_clicks_by_age_gender_logged_in_unique,page_get_directions_clicks_logged_in_by_country_unique,page_get_directions_clicks_logged_in_by_city_unique,page_get_directions_clicks_by_site_logged_in_unique,page_website_clicks_logged_in_unique,page_website_clicks_by_age_gender_logged_in_unique,page_website_clicks_logged_in_by_country_unique,page_website_clicks_logged_in_by_city_unique,page_website_clicks_by_site_logged_in_unique";
        public const string PageUserDemographicsMetrics = "page_fans,page_fans_locale,page_fans_city,page_fans_country,page_fans_gender_age,page_fan_adds,page_fan_adds_unique,page_fans_by_like_source,page_fans_by_like_source_unique,page_fan_removes,page_fan_removes_unique,page_fans_by_unlike_source_unique";
        public const string PageContentMetrics = "page_consumptions_by_consumption_type,page_tab_views_login_top_unique,page_tab_views_login_top,page_tab_views_logout_top";
        public const string PageViewsMetrics = "page_views_total,page_views_logout,page_views_logged_in_total,page_views_logged_in_unique,page_views_external_referrals,page_views_by_profile_tab_total,page_views_by_profile_tab_logged_in_unique,page_views_by_internal_referer_logged_in_unique,page_views_by_site_logged_in_unique,page_views_by_age_gender_logged_in_unique,page_views,page_views_unique,page_views_login,page_views_login_unique,page_visits_logged_in_by_referers_unique";
        public const string PageVideoViewsMetrics = "page_video_views,page_video_views_paid,page_video_views_organic,page_video_views_by_paid_non_paid,page_video_views_autoplayed,page_video_views_click_to_play,page_video_views_unique,page_video_repeat_views,page_video_views_10s,page_video_views_10s_paid,page_video_views_10s_organic,page_video_views_10s_autoplayed,page_video_views_10s_click_to_play,page_video_views_10s_unique,page_video_views_10s_repeat,page_video_view_time";
        public const string PagePostsMetrics = "page_posts_impressions,page_posts_impressions_unique,page_posts_impressions_paid,page_posts_impressions_paid_unique,page_posts_impressions_organic,page_posts_impressions_organic_unique,page_posts_impressions_viral,page_posts_impressions_viral_unique,page_posts_impressions_frequency_distribution";
        public const string PagePostImpressionsMetrics = "post_impressions,post_impressions_unique,post_impressions_paid,post_impressions_paid_unique,post_impressions_fan,post_impressions_fan_unique,post_impressions_fan_paid,post_impressions_fan_paid_unique,post_impressions_organic,post_impressions_organic_unique,post_impressions_viral,post_impressions_viral_unique,post_impressions_nonviral,post_impressions_nonviral_unique,post_impressions_by_story_type,post_impressions_by_story_type_unique";
        public const string PagePostEngagementMetrics = "post_negative_feedback,post_negative_feedback_unique,post_negative_feedback_by_type,post_negative_feedback_by_type_unique,post_engaged_fan";
        public const string PagePostReactionsMetrics = "post_reactions_by_type_total,post_engaged_users";
        public const string PageVideoPostsMetrics = "post_video_avg_time_watched,post_video_complete_views_organic,post_video_complete_views_organic_unique,post_video_complete_views_paid,post_video_complete_views_paid_unique,post_video_retention_graph,post_video_retention_graph_clicked_to_play,post_video_retention_graph_autoplayed,post_video_views_organic,post_video_views_organic_unique,post_video_views_paid,post_video_views_paid_unique,post_video_length,post_video_views,post_video_views_unique,post_video_views_autoplayed,post_video_views_clicked_to_play,post_video_views_10s,post_video_views_10s_unique,post_video_views_10s_autoplayed,post_video_views_10s_clicked_to_play,post_video_views_10s_organic,post_video_views_10s_paid,post_video_views_10s_sound_on,post_video_views_sound_on,post_video_view_time,post_video_view_time_organic,post_video_view_time_by_age_bucket_and_gender,post_video_view_time_by_region_id,post_video_views_by_distribution_type,post_video_view_time_by_distribution_type,post_video_view_time_by_country_id";
        public const string PagePostIdsMetrics = "id";

        public static Dictionary<string, string> Metrics = new Dictionary<string, string>()
        {
            {PagePostStoriesAndPeopleTalkingAboutThis, PagePostStoriesAndPeopleTalkingAboutThisMetrics},
            {PageImpressions,PageImpressionsMetrics },
            {PageEngagement,PageEngagementMetrics },
            {PageReactions,PageReactionsMetrics },
            {PageCtaClicks,PageCtaClicksMetrics },
            {PageUserDemographics,PageUserDemographicsMetrics },
            {PageContent,PageContentMetrics },
            {PageViews,PageViewsMetrics },
            {PageVideoViews,PageVideoViewsMetrics },
            {PagePosts,PagePostsMetrics },
            {PagePostImpressions,PagePostImpressionsMetrics },
            {PagePostEngagement,PagePostEngagementMetrics },
            {PagePostReactions,PagePostReactionsMetrics },
            {PageVideoPosts,PageVideoPostsMetrics },
            {PagePostIds,PagePostIdsMetrics }
        };

    }
}
