using Ninject;
using RedditCore.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RedditCore.Http
{
    internal class UrlFinder : IUrlFinder
    {
        private Regex regex = new Regex(@"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILog log;

        [Inject]
        public UrlFinder(ILog log)
        {
            this.log = log;
        }

        public IList<Uri> FindUrls(string input)
        {
            if(String.IsNullOrEmpty(input))
            {
                return new List<Uri>();
            }

            var results = new List<Uri>();
            foreach (Match match in regex.Matches(input))
            {
                try
                {
                    // The regex matches periods at the end of URLs when the URL ends a sentence.  We do not want that.
                    var url = new Uri(match.Value.TrimEnd('.', ',', ')', ']'));

                    results.Add(url);
                }
                catch (Exception)
                {
                    log.Warning($"Error parsing URL {match.Value}");
                }
            }

            return results;
        }
    }
}
