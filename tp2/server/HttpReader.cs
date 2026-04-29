using shared;

namespace Server;

public static class HttpReader
{
    public static async Task<Dictionary<HttpLabels, string>> ReadHttpHeaders(StreamReader reader)
    {
        Dictionary<HttpLabels, string> headers = new();

        var route = await reader.ReadLineAsync();

        var (method, url) = ParseHeader(route);

        if (method == HttpLabels.Unknown) throw new ArgumentException("405");

        headers[method] = url;

        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(line)) break;

            var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);

            if (parts.Length < 2) throw new ArgumentException("400");

            var label = HttpLabelsMap.GetLabel(parts[0]);
            headers[label] = parts[1];
        }

        return headers;
    }

    public static async Task<string> ReadBody(StreamReader reader, int contentLength = 0)
    {
        if (contentLength == 0) return string.Empty;

        var buffer = new char[contentLength];
        var read = 0;

        while (read < contentLength) read += await reader.ReadAsync(buffer, read, contentLength - read);

        return Uri.UnescapeDataString(new string(buffer).Replace('+', ' '));
    }

    private static (HttpLabels, string) ParseHeader(string? header)
    {
        if (string.IsNullOrEmpty(header)) throw new ArgumentException("400");

        var routeParts = header.Split(' ', 3, StringSplitOptions.TrimEntries);
        if (routeParts.Length < 3) throw new ArgumentException("400");

        var method = HttpLabelsMap.GetLabel(routeParts[0]);
        var url = routeParts[1];

        if (routeParts[2] != "HTTP/1.1") throw new ArgumentException("400");

        return (method, url);
    }
}