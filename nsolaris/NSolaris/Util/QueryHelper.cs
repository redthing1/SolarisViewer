using System.Text;
using System.Web;

namespace NSolaris.Util;

public static class QueryHelper {
    /// <summary>
    /// add query parameters to a url from a dictionary
    /// </summary>
    /// <param name="url"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string BuildUrl(string url, Dictionary<string, string> query) {
        var urlSb = new StringBuilder(url);
        var querySuffixSb = new StringBuilder();
        foreach (var (key, value) in query) {
            querySuffixSb.Append($"&{key}={HttpUtility.UrlEncode(value)}");
        }

        if (querySuffixSb.Length > 0) {
            var querySuffix = querySuffixSb.ToString();
            // replace first & with ?
            querySuffix = querySuffix.Remove(0, 1).Insert(0, "?");
            urlSb.Append(querySuffix);
        }

        return urlSb.ToString();
    }
}