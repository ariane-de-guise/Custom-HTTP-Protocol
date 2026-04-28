using System.Net.Sockets;
using System.Text;
using shared;

namespace Server;

public static class HttpReader
{
    public static async Task<Dictionary<HttpLabels, string>> ReadHttpHeaders(StreamReader reader)
    {
        Dictionary<HttpLabels, string> headers = new();
        
        string? route = await reader.ReadLineAsync();
        Console.WriteLine(route);
        
        (HttpLabels method, string url) = ParseHeader(route);
        
        if (method == HttpLabels.Unknown)
        {
            throw new ArgumentException("405");
        }
        
        headers[method] = url;
        
        while (true)
        {
            string? line = await reader.ReadLineAsync();
            Console.WriteLine(line);

            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            
            string[] parts = line.Split(':', 2, StringSplitOptions.TrimEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException("400");
            }

            HttpLabels label = HttpLabelsMap.GetLabel(parts[0]);
            headers[label] = parts[1];
        }
        
        return headers;
    }
    
    public static async Task<string> ReadBody(StreamReader reader, int contentLength = 0)
    {
        if (contentLength == 0)
        {
            return string.Empty;
        }

        char[] buffer = new char[contentLength];
        int read = 0;

        while (read < contentLength)
        {
            read += await reader.ReadAsync(buffer, read, contentLength - read);
        }

        return new string(buffer);
    }

    private static (HttpLabels, string) ParseHeader(string? header)
    {
        if (String.IsNullOrEmpty(header))
        {
            throw new ArgumentException("400");
        }
        
        string[] routeParts = header.Split(' ', 3, StringSplitOptions.TrimEntries);
        if (routeParts.Length < 3) 
        {
            throw new ArgumentException("400");
        }
        
        HttpLabels method = HttpLabelsMap.GetLabel(routeParts[0]);
        string url = routeParts[1];
        
        if (routeParts[2] != "HTTP/1.1")
        {
            throw new ArgumentException("400");
        }
        
        return (method, url);
    }
}