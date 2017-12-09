using System.Collections.Generic;

namespace RedditCore.SocialGist.Model
{

    public class SearchRequestActual
    {
        public string query { get; set; }
        public string query_id { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
        public string filter_post { get; set; }
        public string sort_mode { get; set; }
        public string mode { get; set; }
        public string filter_language { get; set; }
        public string filter_langid { get; set; }
        public string filter_country { get; set; }
        public string filter_region { get; set; }
        public string filter_site { get; set; }
        public string filter_site_key { get; set; }
        public string dn { get; set; }
        public string filter_forum { get; set; }
        public string filter_thread { get; set; }
        public string isthread { get; set; }
        public string filter_author { get; set; }
        public string filter_date_from { get; set; }
        public string filter_date_to { get; set; }
        public string filter_inserted_from { get; set; }
        public string filter_inserted_to { get; set; }
        public string body { get; set; }
        public string highlight { get; set; }
        public string match_mode { get; set; }
        public string max_matches { get; set; }
        public string key { get; set; }
        public string source { get; set; }
        public string action { get; set; }
        public string rt { get; set; }
        public string group_by { get; set; }
        public string group_mode { get; set; }
        public string group_date { get; set; }
        public string filter_timestamp_from { get; set; }
        public string filter_timestamp_to { get; set; }
        public string callback { get; set; }
        public string mblog { get; set; }
        public string gnip { get; set; }
        public string debug { get; set; }
    }

    public class SearchRequest
    {
        public string action { get; set; }
        public string dn { get; set; }
        public string filter_date_from { get; set; }
        public string filter_domain { get; set; }
        public string group_mode { get; set; }
        public string highlight { get; set; }
        public string key { get; set; }
        public string match_mode { get; set; }
        public string max_matches { get; set; }
        public string mblog { get; set; }
        public string query { get; set; }
        public string rt { get; set; }
        public string sort_mode { get; set; }
        public string source { get; set; }
        public string st { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
        public string UserIP { get; set; }
        public SearchRequestActual Actual { get; set; }
    }

    public class SearchMatch : IMatch
    {
        public object Id { get; set; }
        public string ForumId { get; set; }
        public string ThreadId { get; set; }
        public string Url { get; set; }
    }

    public class SearchMatches : IMatches<SearchMatch>
    {
        public List<SearchMatch> Match { get; set; }
    }

    public class SearchResponse : IResponse<SearchMatches, SearchMatch> 
    {
        public int RequestsUsed { get; set; }
        public string RequestsLimit { get; set; }
        public string ConcurrentRequests { get; set; }
        public string RequestsInQueue { get; set; }
        public string ActiveConcurrentRequestsAllowed { get; set; }
        public string MaxConcurrentRequests { get; set; }
        public SearchRequest Request { get; set; }
        public SearchMatches Matches { get; set; }
        public string TotalFound { get; set; }
        public string Total { get; set; }
        public string SearchTime { get; set; }
        public Error Error { get; set; }
    }

    public class SearchApiResponse : IApiResponse<SearchResponse, SearchMatches, SearchMatch>
    {
        public SearchResponse response { get; set; }
    }
}
