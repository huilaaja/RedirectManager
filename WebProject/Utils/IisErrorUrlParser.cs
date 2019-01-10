using System;
using System.Text.RegularExpressions;
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
            //example: https://www.example-domain.com/error/http404?404;https://example-domain:80/some/path?query=string
            Regex urlRegex = new Regex($"\\?404;(\\w+):\\/\\/(.*):(\\d?)(\\d?)(\\d?)(\\d?)(\\d?)\\/(?<relativePath>.*)", RegexOptions.Compiled);
            string relativePath = urlRegex.Match(iisHttpErrorUri.Query)?.Groups["relativePath"]?.Value;
            return relativePath != null ? "/" + relativePath : null;
        }
    }
}
