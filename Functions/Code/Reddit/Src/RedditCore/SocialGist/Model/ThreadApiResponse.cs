using System.Collections.Generic;
using Newtonsoft.Json;
using RedditCore.DataModel;

namespace RedditCore.SocialGist.Model
{
    public class ThreadApiResponse
    {

        [JsonProperty("requests")]
        public int Requests { get; set; }

        [JsonProperty("reties")]
        public int Retries { get; set; }

        [JsonProperty("retry_fatals")]
        public int RetryFatals { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("comments_count")]
        public int CommentsCount { get; set; }

        [JsonProperty("post")]
        public ResponsePost Post { get; set; }

        [JsonProperty("comments")]
        public IList<ResponseComment> Comments { get; set; }
    }

    public class Media
    {
        [JsonProperty("oembed")]
        public Oembed Oembed { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Oembed
    {
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("thumbnail_height")]
        public int ThumbnailHeight { get; set; }

        [JsonProperty("thumbnail_width")]
        public int ThumbnailWidth { get; set; }

        [JsonProperty("provider_url")]
        public string ProviderUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("provider_name")]
        public string ProviderName { get; set; }
    }

    public class Source
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Resolution
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Variants
    {
    }

    public class Image
    {
        [JsonProperty("source")]
        public Source Source { get; set; }
        [JsonProperty("resolutions")]
        public IList<Resolution> Resolutions { get; set; }
        [JsonProperty("variants")]
        public Variants Variants { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Preview
    {
        [JsonProperty("images")]
        public IList<Image> Images { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }

    public class SecureMediaEmbed
    {
    }

    public class MediaEmbed
    {
    }


    public class ResponsePost : IDocument
    {
        // IDocument
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Content => Selftext;

        // Other fields
        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("approved_at_utc")]
        public object ApprovedAtUtc { get; set; }

        [JsonProperty("banned_by")]
        public object BannedBy { get; set; }

        [JsonProperty("media_embed")]
        public MediaEmbed MediaEmbed { get; set; }

        [JsonProperty("thumbnail_width")]
        public object ThumbnailWidth { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("selftext_html")]
        public string SelftextHtml { get; set; }

        [JsonProperty("selftext")]
        public string Selftext { get; set; }

        [JsonProperty("likes")]
        public object Likes { get; set; }

        [JsonProperty("suggested_sort")]
        public object SuggestedSort { get; set; }

        [JsonProperty("user_reports")]
        public IList<object> UserReports { get; set; }

        [JsonProperty("secure_media")]
        public object SecureMedia { get; set; }

        [JsonProperty("is_reddit_media_domain")]
        public bool IsRedditMediaDomain { get; set; }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("link_flair_text")]
        public object LinkFlairText { get; set; }

        [JsonProperty("banned_at_utc")]
        public object BannedAtUtc { get; set; }

        [JsonProperty("view_count")]
        public object ViewCount { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("clicked")]
        public bool Clicked { get; set; }

        [JsonProperty("report_reasons")]
        public object ReportReasons { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("num_crossposts")]
        public int NumCrossposts { get; set; }

        [JsonProperty("saved")]
        public bool Saved { get; set; }

        [JsonProperty("can_mod_post")]
        public bool CanModPost { get; set; }

        [JsonProperty("is_crosspostable")]
        public bool IsCrosspostable { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("approved_by")]
        public object ApprovedBy { get; set; }

        [JsonProperty("over_18")]
        public bool Over18 { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("num_comments")]
        public int NumComments { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("subreddit_id")]
        public string SubredditId { get; set; }

        [JsonProperty("hide_score")]
        public bool HideScore { get; set; }

        [JsonProperty("edited")]
        public bool Edited { get; set; }

        [JsonProperty("link_flair_css_class")]
        public object LinkFlairCssClass { get; set; }

        [JsonProperty("author_flair_css_class")]
        public object AuthorFlairCssClass { get; set; }

        [JsonProperty("contest_mode")]
        public bool ContestMode { get; set; }

        [JsonProperty("gilded")]
        public int Gilded { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("downs")]
        public int Downs { get; set; }

        [JsonProperty("brand_safe")]
        public bool BrandSafe { get; set; }

        [JsonProperty("secure_media_embed")]
        public SecureMediaEmbed SecureMediaEmbed { get; set; }

        [JsonProperty("removal_reason")]
        public object RemovalReason { get; set; }

        [JsonProperty("can_gild")]
        public bool CanGild { get; set; }

        [JsonProperty("thumbnail_height")]
        public object ThumbnailHeight { get; set; }

        [JsonProperty("parent_whitelist_status")]
        public string ParentWhitelistStatus { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("spoiler")]
        public bool Spoiler { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("num_reports")]
        public object NumReports { get; set; }

        [JsonProperty("whitelist_status")]
        public string WhitelistStatus { get; set; }

        [JsonProperty("stickied")]
        public bool Stickied { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("author_flair_text")]
        public object AuthorFlairText { get; set; }

        [JsonProperty("quarantine")]
        public bool Quarantine { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("created_utc")]
        public int CreatedUtc { get; set; }

        [JsonProperty("subreddit_name_prefixed")]
        public string SubredditNamePrefixed { get; set; }

        [JsonProperty("distinguished")]
        public object Distinguished { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("upvote_ratio")]
        public double UpvoteRatio { get; set; }

        [JsonProperty("mod_reports")]
        public IList<object> ModReports { get; set; }

        [JsonProperty("is_self")]
        public bool IsSelf { get; set; }

        [JsonProperty("visited")]
        public bool Visited { get; set; }

        [JsonProperty("subreddit_type")]
        public string SubredditType { get; set; }

        [JsonProperty("is_video")]
        public bool IsVideo { get; set; }

        [JsonProperty("ups")]
        public int Ups { get; set; }
    }

    public class ResponseComment : IDocument
    {

        // IDocument
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Content => Body;

        // Other properties
        [JsonProperty("depth")]
        public int Depth { get; set; }

        [JsonProperty("subreddit_id")]
        public string SubredditId { get; set; }

        [JsonProperty("approved_at_utc")]
        public object ApprovedAtUtc { get; set; }

        [JsonProperty("banned_by")]
        public object BannedBy { get; set; }

        [JsonProperty("removal_reason")]
        public object RemovalReason { get; set; }

        [JsonProperty("link_id")]
        public string LinkId { get; set; }

        [JsonProperty("likes")]
        public object Likes { get; set; }

        [JsonProperty("user_reports")]
        public IList<object> UserReports { get; set; }

        [JsonProperty("saved")]
        public bool Saved { get; set; }

        [JsonProperty("banned_at_utc")]
        public object BannedAtUtc { get; set; }

        [JsonProperty("gilded")]
        public int Gilded { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("report_reasons")]
        public object ReportReasons { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("can_mod_post")]
        public bool CanModPost { get; set; }

        [JsonProperty("ups")]
        public int Ups { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("approved_by")]
        public object ApprovedBy { get; set; }

        [JsonProperty("downs")]
        public int Downs { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("edited")]
        public bool Edited { get; set; }

        [JsonProperty("author_flair_css_class")]
        public string AuthorFlairCssClass { get; set; }

        [JsonProperty("collapsed")]
        public bool Collapsed { get; set; }

        [JsonProperty("is_submitter")]
        public bool IsSubmitter { get; set; }

        [JsonProperty("collapsed_reason")]
        public object CollapsedReason { get; set; }

        [JsonProperty("body_html")]
        public string BodyHtml { get; set; }

        [JsonProperty("stickied")]
        public bool Stickied { get; set; }

        [JsonProperty("can_gild")]
        public bool CanGild { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("score_hidden")]
        public bool ScoreHidden { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("subreddit_type")]
        public string SubredditType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("author_flair_text")]
        public string AuthorFlairText { get; set; }

        [JsonProperty("created_utc")]
        public int CreatedUtc { get; set; }

        [JsonProperty("subreddit_name_prefixed")]
        public string SubredditNamePrefixed { get; set; }

        [JsonProperty("controversiality")]
        public double Controversiality { get; set; }

        [JsonProperty("mod_reports")]
        public IList<object> ModReports { get; set; }

        [JsonProperty("num_reports")]
        public object NumReports { get; set; }

        [JsonProperty("distinguished")]
        public object Distinguished { get; set; }
    }
}
