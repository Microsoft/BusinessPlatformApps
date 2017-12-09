using System;
using System.Linq;
using RedditCore.DataModel;
using RedditCore.Logging;
using RedditCore.SocialGist.Model;

namespace RedditCore.SocialGist
{
    internal class ThreadResponseCommentTransformer 
    {
        private readonly IIds ids;
        private readonly ILog log;

        public ThreadResponseCommentTransformer(IIds ids, ILog log)
        {
            this.ids = ids;
            this.log = log;
        }

        public Comment Transform(ResponseComment x)
        {
            var updated = DateTimes.UnixTimeStampToDateTime(x.CreatedUtc);
            var url = $"https://www.reddit.com{x.Permalink}";
            var parentRedditId = ids.ExtractIdFromTypeAndId(x.ParentId);
            var parentUrl = ids.ParentUrlFromChildDetails(
                childUrl: url,
                childId: x.Id,
                parentId: parentRedditId
            );
            var generatedParentId = ids.IdFromUrl(parentUrl);
            var postUrl = ids.PostUrlFromChildDetails(
                childUrl: url,
                childId: x.Id
            );
            var generatedPostId = ids.IdFromUrl(postUrl);

            var id = ids.IdFromUrl(url);

            if (new string[] {parentRedditId, parentUrl, generatedParentId, postUrl, generatedPostId}.Any(
                string.IsNullOrEmpty))
            {
                log.Error(
                    $"One or more of the essential structure providing details was null or empty: [parentRedditId={parentRedditId}], [parentUrl={parentUrl}], [generatedParentId={generatedParentId}], [postUrl={postUrl}], [generatedPostId={generatedPostId}]");
                log.Error("This entry will not be saved to the database!");

                return new InvalidComment() {Id = id};
            }
            else
            {
                return new Comment
                {
                    Id = id,
                    Author = x.Author.Truncate(100),
                    Content = x.Body,
                    Score = x.Score,
                    Controversiality = x.Controversiality,
                    Gilded = x.Gilded,
                    Subreddit = ids.ExtractSubredditIdFromUrl(url) ?? string.Empty,
                    PublishedMonthPrecision = updated.MonthPrecision(),
                    PublishedWeekPrecision = updated.DayOfWeekPrecision(),
                    PublishedDayPrecision = updated.DayPrecision(),
                    PublishedHourPrecision = updated.HourPrecision(),
                    PublishedMinutePrecision = updated.MinutePrecision(),
                    PublishedTimestamp = updated,
                    IngestedTimestamp = DateTime.UtcNow,
                    Url = url,
                    ParentId = generatedParentId,
                    PostId = generatedPostId,
                    ParentUrl = parentUrl,
                    PostUrl = postUrl,
                };
            }
        }
    }
}
