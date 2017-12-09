using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.SocialGist
{
    public class Ids : IIds
    {
        private readonly ILog logger;

        private readonly Regex subredditRegex = new Regex("https?://[w\\.]*reddit\\.com/r/([^/]+)", RegexOptions.Compiled);

        [Inject]
        public Ids(ILog logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public string IdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                logger.Error("The provided url is null or empty");
                return null;
            }
            using (var hash = SHA256.Create())
            {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(url))
                    .Select(item => item.ToString("x2")));
            }
        }

        /// <inheritdoc/>
        public string ParentUrlFromChildDetails(
            string childUrl,
            string childId,
            string parentId
        )
        {
            if (new string[] {childUrl, childId, parentId}.Any(string.IsNullOrEmpty))
            {
                logger.Error($"An argument is null or empty; [childUrl={childUrl}], [childId={childId}], [parentId={parentId}]");
                return null;
            }

            if (!childUrl.Contains(childId))
            {
                logger.Error($"The childUrl [{childUrl}] provided somehow does not contain the childId [{childId}] provided");
                return null;
            }
            if (childUrl.Contains(parentId))
            {
                // this means the URL already contains the parentId.  The only way this can happen is if the parentId is the post itself.
                logger.Info($"The childUrl [{childUrl}] provided contains the parentId [{parentId}] already, indicating that the parent is the post itself");
                return PostUrlFromChildDetails(childUrl, childId);
            }
            return childUrl.Replace(childId, parentId);
        }

        /// <inheritdoc />>
        public string ExtractSubredditIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                logger.Info("URL is null or empty.  No subreddit ID found.");
                return null;
            }

            var match = subredditRegex.Match(url);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                logger.Info($"No subreddit found in URL {url}");
                return null;
            }
        }

        /// <inheritdoc/>
        public string ExtractIdFromTypeAndId(string typeAndId)
        {
            if (string.IsNullOrEmpty(typeAndId))
            {
                logger.Error("typeAndId is null or empty");
                return null;
            }

            var split = typeAndId.Split('_');
            if (split.Length != 2)
            {
                logger.Error($"Our assumption is that the typeAndId takes the form of t<#>_<base36 string>.  This provided typeAndId did not follow that pattern: {typeAndId}");
                return null;
            }
            var id = split[1];
            if (string.IsNullOrWhiteSpace(id))
            {
                logger.Error($"After splitting {typeAndId} on _, the second part was an empty string; returning null");
                return null;
            }
            return id;
        }

        /// <inheritdoc/>
        public string PostUrlFromChildDetails(
            string childUrl,
            string childId
        )
        {
            if (new string[] {childUrl, childId}.Any(string.IsNullOrEmpty))
            {
                logger.Error($"An argument is null or empty; [childUrl={childUrl}], [childId={childId}]");
                return null;
            }

            var idTrailingSlash = $"{childId}/";

            if (!childUrl.Contains(childId))
            {
                return null;
            }
            return childUrl.Replace(idTrailingSlash, "").Replace(childId, "");
        }
    }
}