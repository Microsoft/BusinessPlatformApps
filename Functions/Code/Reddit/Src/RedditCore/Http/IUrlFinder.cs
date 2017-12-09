using System;
using System.Collections.Generic;

namespace RedditCore.Http
{
    public interface IUrlFinder
    {
        IList<Uri> FindUrls(string input);
    }
}
