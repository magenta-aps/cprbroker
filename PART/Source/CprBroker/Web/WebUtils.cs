using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CprBroker.Web
{
    public static class WebUtils
    {
        public static string UrlWithoutQuery(string url)
        {
            var full = url;
            var ind = full.IndexOf('?');
            var urlWithoutQuery = ind >= 0 ? full.Substring(0, ind) : full;
            return urlWithoutQuery;
        }

        public static string CreateLink(string name, string value, Uri baseUrl, params string[] namesToRemove)
        {
            var parameters = HttpUtility.ParseQueryString(baseUrl.Query);

            if (string.IsNullOrEmpty(value))
                parameters.Remove(name);
            else
                parameters[name] = value;

            foreach (var nameToRemove in namesToRemove)
                parameters.Remove(nameToRemove);

            var urlWithoutQuery = UrlWithoutQuery(baseUrl.ToString());
            if (parameters.Count > 0)
                return urlWithoutQuery + "?" + parameters.ToString();
            else
                return urlWithoutQuery;
        }
    }
}