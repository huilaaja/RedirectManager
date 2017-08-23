using System;
using System.Web;
namespace WebProject.Utils
{
    public static class IisErrorUrlParser
    {
        public static Uri GetOriginalUrl(HttpRequestBase request, int statusCode)
        {
            return GetOriginalUrl(request.Url, statusCode);
        }

        public static Uri GetOriginalUrl(Uri iisHttpErrorUri, int statusCode)
        {
            string iisErrorUrlPrefix = $"?{statusCode};"; //example: "?404;"
            string absoluteHost = $"{iisHttpErrorUri.Scheme}:{iisHttpErrorUri.Host}:{iisHttpErrorUri.Port}/";
            string relativeUrl = iisHttpErrorUri.Query
                                .Replace(iisErrorUrlPrefix, "") //remove prefix: "?404;"
                                .Replace(absoluteHost, "/"); //make relative
            return new Uri(iisHttpErrorUri, relativeUrl); //make absolute
        }
    }
}
