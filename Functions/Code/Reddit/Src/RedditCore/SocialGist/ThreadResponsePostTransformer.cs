using System;
using RedditCore.DataModel;
using RedditCore.SocialGist.Model;

namespace RedditCore.SocialGist
{
    internal class ThreadResponsePostTransformer 
    {
        private readonly IIds ids;

        public ThreadResponsePostTransformer(IIds ids)
        {
            this.ids = ids;
        }

        public Post Transform(ResponsePost x)
        {
            var updated = DateTimes.UnixTimeStampToDateTime(x.CreatedUtc);
            var possibleMediaPreviewUrl = GetMediaPreviewUrl(x);
            return new Post
            {
                Id = ids.IdFromUrl(x.Url),
                Author = x.Author.Truncate(100),
                Content = x.Selftext,
                Score = x.Score,
                Controversiality = null,
                Gilded = x.Gilded,
                Title = x.Title.Truncate(200),
                Subreddit = ids.ExtractSubredditIdFromUrl(x.Url) ?? string.Empty,
                PublishedMonthPrecision = updated.MonthPrecision(),
                PublishedWeekPrecision = updated.DayOfWeekPrecision(),
                PublishedDayPrecision = updated.DayPrecision(),
                PublishedHourPrecision = updated.HourPrecision(),
                PublishedMinutePrecision = updated.MinutePrecision(),
                PublishedTimestamp = updated,
                IngestedTimestamp = DateTime.UtcNow,
                Url = x.Url,
                MediaPreviewUrl = possibleMediaPreviewUrl,
            };
        }

        /// <summary>
        /// There are many media URLs we may want to return for this preview URL and this method is a simple abstraction for us to swap out which we prefer
        /// without making the Process method more complex than it has to be.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        internal static string GetMediaPreviewUrl(ResponsePost thread)
        {
            /*
             * there are at least two paths we can follow for this:
             * 1: we can use thread.media.oembed.thumbnail_url.  The problem is this "thumbnail" can be any resolution - including my example, which was 480x270
             * 2: we can use thread.preview.images[0].source.url.  This url can also be any resolution - including my example, which was 1366x766
             * 
             * We can also return whichever exists first.  However, they may also be super large, which is super wasteful when we're just going to have to put height and width restrictions
             * on their display anyway.  
             * 
             * Maybe instead we should try to get both, and if both exist, attempt to return the smallest.
             */
            var oembed = thread?.Media?.Oembed;
            var source = (
                    thread?.Preview?.Images != null 
                    && thread?.Preview?.Images.Count > 0
                ) 
                ? thread.Preview.Images[0]?.Source 
                : null;
            if (oembed?.ThumbnailUrl == null || source?.Url == null)
            {
                return oembed?.ThumbnailUrl ?? source?.Url; // could return null if both are null
            }

            // at this point, both of these should be valid.

            // so, it's POSSIBLE that aspect ratio between the two images could change.  it shouldn't, as they are supposed to just be resized versions, but let's just say they do
            // take the total area of the image as the value to judge on smallness, then return the corresponding image url.
            if ( source.Height * source.Width < oembed.ThumbnailHeight * oembed.ThumbnailWidth)
            {
                return source.Url;
            }
            else
            {
                return oembed.ThumbnailUrl;
            }
            
        }
    }
}
