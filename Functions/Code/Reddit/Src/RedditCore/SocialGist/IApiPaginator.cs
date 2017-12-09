using System.Collections.Generic;
using System.Threading.Tasks;
using RedditCore.SocialGist.Model;

namespace RedditCore.SocialGist
{
    public interface IApiPaginator
    {
        Task<List<TMatch>> PageThroughCallResults<TApiResponse, TResponse, TMatches, TMatch>(
            string baseUrl,
            Dictionary<string, object> parameters,
            int resultLimitPerPage
        )
            where TApiResponse : class, IApiResponse<TResponse, TMatches, TMatch>
            where TResponse : IResponse<TMatches, TMatch>
            where TMatches : IMatches<TMatch>
            where TMatch : IMatch;

    }
}