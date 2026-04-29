using shared;

namespace Server;

public static class UrlParser
{
    public static (Func<string, bool, string, string> method, string param) Parse(string url, HttpLabels label)
    {
        var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if ((label == HttpLabels.Get && url.Contains("script")) || url.Contains("style")) return GetUrl(url);

        if (parts.Length == 0)
        {
            if (label != HttpLabels.Get) throw new ArgumentException("405");
            return (Views.Index, "");
        }

        if (parts.Length < 2 || parts[0] != "api" || parts[1] != "messages" || parts.Length > 3)
            throw new ArgumentException("404");

        var id = "";

        if (parts.Length == 3) id = parts[2];

        switch (label)
        {
            case HttpLabels.Get:
                return (Views.Show, id);
            case HttpLabels.Post:
                return (Views.Post, id);
            case HttpLabels.Delete:
                return (Views.Delete, id);
            case HttpLabels.Patch:
                return (Views.Patch, id);
            case HttpLabels.Put:
                return (Views.Patch, id);
            default:
                throw new ArgumentException("405");
        }
    }

    private static (Func<string, bool, string, string> method, string param) GetUrl(string url)
    {
        if (url.Contains("script")) return (Views.GetJs, "");

        if (url.Contains("style")) return (Views.GetCss, "");

        throw new ArgumentException("404");
    }
}