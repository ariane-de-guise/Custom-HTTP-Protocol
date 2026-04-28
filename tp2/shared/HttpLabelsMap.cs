namespace shared;

public class HttpLabelsMap
{
    private static readonly Dictionary<string, HttpLabels> Map = new()
    {
        // Verbs
        { "get", HttpLabels.Get },
        { "put", HttpLabels.Put },
        { "post", HttpLabels.Post },
        { "delete", HttpLabels.Delete },
        { "patch", HttpLabels.Patch },

        // General Headers
        { "cache-control", HttpLabels.CacheControl },
        { "connection", HttpLabels.Connection },
        { "date", HttpLabels.Date },
        { "transfer-encoding", HttpLabels.TransferEncoding },
        { "via", HttpLabels.Via },

        // Request Headers
        { "host", HttpLabels.Host },
        { "user-agent", HttpLabels.UserAgent },
        { "accept", HttpLabels.Accept },
        { "accept-language", HttpLabels.AcceptLanguage },
        { "accept-encoding", HttpLabels.AcceptEncoding },
        { "authorization", HttpLabels.Authorization },
        { "cookie", HttpLabels.Cookie },
        { "referer", HttpLabels.Referer },
        { "origin", HttpLabels.Origin },

        // Response Headers
        { "server", HttpLabels.Server },
        { "set-cookie", HttpLabels.SetCookie },
        { "location", HttpLabels.Location },

        // Entity Headers
        { "content-length", HttpLabels.ContentLength },
        { "content-type", HttpLabels.ContentType },
        { "content-language", HttpLabels.ContentLanguage },
        { "content-encoding", HttpLabels.ContentEncoding }
    };

    public static HttpLabels GetLabel(string headerName)
    {
        if (string.IsNullOrEmpty(headerName))
        {
            return HttpLabels.Unknown;
        }

        bool found = Map.TryGetValue(headerName.ToLower(), out HttpLabels label);

        return found ? label : HttpLabels.Unknown;
    }
}