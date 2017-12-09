namespace RedditCore.SocialGist
{
    public interface IIds
    {
        /// <summary>
        /// Hashes the provided URL with a SHA256 hashing algorithm.  Returns null if the provided url is null or empty.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string IdFromUrl(string url);

        /// <summary>
        /// Attempts to construct the Parent URL for a given comment URL, comment ID, and parentId.
        /// 
        /// As the parentId could be the post itself, we have to do some special logic around that.  Returns a null if we cannot generate the parent URL.  
        /// </summary>
        /// <param name="childUrl">Should not be empty or null</param>
        /// <param name="childId">Should not be empty or null</param>
        /// <param name="parentId">Should not be empty or null.  Should be only the ID portion of the TypeAndId.  See ExtractIdFromTypeAndId for more details.</param>
        /// <returns>Parent URL or null if we cannot safely calculate it.</returns>
        string ParentUrlFromChildDetails(
            string childUrl,
            string childId,
            string parentId
        );

        /// <summary>
        /// Gets the subreddit ID from the post or comment URL.  Subreddit ID is immutable and is found in the URL.  Subreddit name may be changed and is not found in the URL.
        /// </summary>
        /// <param name="url">URL of the Reddit post or comment</param>
        /// <returns>Subreddit ID (may or may not match the subreddit name)</returns>
        string ExtractSubredditIdFromUrl(string url);

        /// <summary>
        /// Attempts to extract the reddit ID from the given TypeAndId.  This TypeAndId basically takes the form of t#_base36string.
        /// If the typeAndId provided are null or empty, or don't have this form, we log the error and return null.  This id is eventually used 
        /// for our parent URL derivation.  It is fed by the "parent_id" directly on a ThreadMatch object.  The ThreadMatch.BR_API.ThreadId has already parsed this out, 
        /// which is predominantly used as the PostId.
        /// </summary>
        /// <param name="typeAndId"></param>
        /// <returns>The id from the TypeAndId passed in</returns>
        string ExtractIdFromTypeAndId(string typeAndId);

        /// <summary>
        /// Attempts to extract the Post URL from the child details.  It does this by removing the childId (and any possible trailing whitespace) from the childUrl.
        /// </summary>
        /// <param name="childUrl"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        string PostUrlFromChildDetails(
            string childUrl,
            string childId
        );
    }
}
