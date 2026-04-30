using System.Net.Sockets;
using System.Text;

namespace Server;

/// <summary>
///     Classe utilitaire pour envoyer des réponses HTTP.
/// </summary>
public static class HttpWriter
{
    /// <summary>
    ///     Fonction utilitaire pour envoyer des réponses générales HTTP.
    /// </summary>
    /// <param name="stream">Flux réseau du client.</param>
    /// <param name="contentType">Le type du body de la réponse envoyée.</param>
    /// <param name="statusCode">L'état de la requête.</param>
    /// <param name="body">Le body de la réponse (non obligatoire)</param>
    public static async Task WriteResponseAsync(NetworkStream stream, string contentType, int statusCode, string body)
    {
        StringBuilder sb = new();
        sb.Append($"HTTP/1.1 {statusCode} {StatusCodeHelper.GetReasonPhrase(statusCode)}\r\n");
        sb.Append($"Content-Type: {contentType}\r\n");
        if (!string.IsNullOrEmpty(body)) sb.Append($"Content-Length: {body.Length}\r\n");
        sb.Append("Connection: close\r\n");
        sb.Append("\r\n");

        if (!string.IsNullOrEmpty(body)) sb.Append(body);

        var responseBytes = Encoding.UTF8.GetBytes(sb.ToString());
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }

    /// <summary>
    ///     Fonction utilitaire pour envoyer une redirection HTTP 303 See Other.
    /// </summary>
    /// <param name="stream">Flux réseau du client.</param>
    /// <param name="location">L'url à lequel on souhaite rediriger.</param>
    public static async Task WriteRedirectAsync(NetworkStream stream, string location)
    {
        var response =
            "HTTP/1.1 303 See Other\r\n" +
            $"Location: {location}\r\n" +
            "Connection: close\r\n" +
            "\r\n";

        var bytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(bytes);
    }
}