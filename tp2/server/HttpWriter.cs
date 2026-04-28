using System.Net.Sockets;
using System.Text;

namespace Server;

public class HttpWriter
{
    public static async Task WriteResponseAsync(NetworkStream stream, string contentType = "application/json", int statusCode = 200, string body = "")
    {
        StringBuilder sb = new();
        sb.Append($"HTTP/1.1 {statusCode} {ErrorsHelper.GetReasonPhrase(statusCode)}\r\n");
        sb.Append($"Content-Type: {contentType}\r\n");
        if (!string.IsNullOrEmpty(body))
        {
            sb.Append($"Content-Length: {body.Length}\r\n");
        }
        sb.Append("Connection: close\r\n");
        sb.Append("\r\n");
        
        if (!string.IsNullOrEmpty(body))
        {
            sb.Append(body);
        }
        
        byte[] responseBytes = Encoding.UTF8.GetBytes(sb.ToString());
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }
    
    public static async Task WriteRedirectAsync(NetworkStream stream, string location)
    {
        string response =
            "HTTP/1.1 303 See Other\r\n" +
            $"Location: {location}\r\n" +
            "Connection: close\r\n" +
            "\r\n";

        byte[] bytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(bytes);
    }
}