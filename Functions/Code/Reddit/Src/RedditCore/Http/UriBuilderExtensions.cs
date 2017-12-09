using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RedditCore.Http
{
    public static class UriBuilderExtensions
    {
        public static UriBuilder AddQueryParam(this UriBuilder builder, string key, int value)
        {
            return AddQueryParam(builder, key, value.ToString());
        }

        public static UriBuilder AddQueryParam(this UriBuilder builder, string key, string value)
        {
            var stringBuilder = new StringBuilder(builder.Query);


            if (stringBuilder.Length != 0)
            {
                if (stringBuilder[0] == '?')
                    stringBuilder.Remove(0, 1);

                stringBuilder.Append('&');
            }

            builder.Query = stringBuilder
                .Append(HttpUtility.UrlEncode(key))
                .Append('=')
                .Append(HttpUtility.UrlEncode(value))
                .ToString();

            return builder;
        }

        public static UriBuilder QueryParamsFromDictionary(
            this UriBuilder builder, 
            Dictionary<string, Object> parameters
        )
        {
            if(builder == null)
            {
                throw new ArgumentNullException("builder", "UriBuilder cannot be null");
            }

            if (parameters != null)
            {
                foreach (KeyValuePair<string, Object> kvp in parameters)
                {
                    if (kvp.Value != null)
                    {
                        AddQueryParam(builder, kvp.Key, kvp.Value.ToString());
                    }
                }
            }

            return builder;
        }
    }
}
