using System.Collections.Generic;
using System.Threading.Tasks;
using RedditCore.DataModel;
using RedditCore.SocialGist.Model;

namespace RedditCore.SocialGist
{
    public interface ISocialGist
    {
        Task<ThreadApiResponse> CommentsForThread(SocialGistPostId post);

        Task<SortedSet<SocialGistPostId>> MatchesForQuery(
            string query,
            long? startUnixTime = null
        );

        int ResultLimitPerPage { get; set; }
        int MaximumResultsPerSearch { get; set; }
    }
}
