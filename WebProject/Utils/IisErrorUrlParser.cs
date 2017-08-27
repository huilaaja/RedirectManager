using System;
using System.Web;
namespace WebProject.Utils
{
    public static class IisErrorUrlParser
    {
        public static string GetOriginalRelativePath(HttpRequestBase request, int statusCode)
        {
            return GetOriginalRelativePath(request.Url, statusCode);
        }

        public static string GetOriginalRelativePath(Uri iisHttpErrorUri, int statusCode)
        {
            string iisErrorUrlPrefix = $"?{statusCode};"; //example: "?404;"
            string absoluteHost = $"{iisHttpErrorUri.Scheme}:{iisHttpErrorUri.Host}:{iisHttpErrorUri.Port}/";
            string relativeUrl = iisHttpErrorUri.Query
                                .Replace(iisErrorUrlPrefix, "") //remove prefix: "?404;"
                                .Replace(absoluteHost, "/"); //make relative
            return relativeUrl;
        }
    }
}
