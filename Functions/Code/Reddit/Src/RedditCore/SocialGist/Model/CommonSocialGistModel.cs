using System.Collections.Generic;

namespace RedditCore.SocialGist.Model
{
    public interface IMatch { }

    public interface IMatches<TMatch> 
        where TMatch : IMatch
    {
        List<TMatch> Match { get; set; }
    }

    public interface IResponse<TMatches, TMatch>
        where TMatches : IMatches<TMatch>
        where TMatch : IMatch
    {
        Error Error { get; set; }
        TMatches Matches { get; set; }
        string Total { get; set; }
    }

    public interface IApiResponse<TResponse, TMatches, TMatch>
        where TResponse : IResponse<TMatches, TMatch>
        where TMatches : IMatches<TMatch>
        where TMatch : IMatch 
    {
        TResponse response { get; set; }
    }

 }