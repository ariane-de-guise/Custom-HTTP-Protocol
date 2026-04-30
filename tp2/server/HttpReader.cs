using shared;

namespace Server;

/// <summary>
///     Classe utilitaire pour lire des requêtes HTTP/1.1.
/// </summary>
public static class HttpReader
{
    /// <summary>
    ///     Fonction utiliaitre pour lire les en-têtes d'une requête HTTP/1.1 à partir d'un StreamReader.
    ///     La fonction s'arrête lorsque \r\n\r\n est rencontré, indiquant la fin des en-têtes.
    /// </summary>
    /// <param name="reader">StreamReader associé au flux du TcpClient.</param>
    /// <returns>Dictionnaire liant les types de header à leurs contenus.</returns>
    /// <exception cref="ArgumentException">
    ///     400 - Si la méthode HTTP est inconnue ou si les en-têtes sont mal formés.
    /// </exception>
    public static async Task<Dictionary<HttpLabels, string>> ReadHttpHeaders(StreamReader reader)
    {
        Dictionary<HttpLabels, string> headers = new();

        var route = await reader.ReadLineAsync();

        var (method, url) = ParseRoute(route);

        if (method == HttpLabels.Unknown) throw new ArgumentException("400");

        headers[method] = url;

        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(line)) break;

            var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);

            if (parts.Length != 2) throw new ArgumentException("400");

            var label = HttpLabelsMap.GetLabel(parts[0]);
            headers[label] = parts[1];
        }

        return headers;
    }

    /// <summary>
    ///     Fonction utilitaire pour lire le body d'une requête HTTP/1.1.
    /// </summary>
    /// <param name="reader">StreamReader associé au flux du TcpClient.</param>
    /// <param name="contentLength">Longueur du body, 0 par défaut.</param>
    /// <returns>La string du body qui a été parsed.</returns>
    public static async Task<string> ReadBody(StreamReader reader, int contentLength = 0)
    {
        if (contentLength == 0) return string.Empty;

        var buffer = new char[contentLength];
        var read = 0;

        while (read < contentLength) read += await reader.ReadAsync(buffer, read, contentLength - read);

        return Uri.UnescapeDataString(new string(buffer).Replace('+', ' '));
    }

    /// <summary>
    ///     Fonction utilitaire pour convertir la route (url & méthode).
    /// </summary>
    /// <param name="header">Première ligne d'une requête HTTP.</param>
    /// <returns>Tuple contenant la méthode (GET, POST, PATCH...) et l'url.</returns>
    /// <exception cref="ArgumentException">
    ///     400 - Si le header est null, vide ou mal formé.
    ///     405 - Si l'url ne supporte pas la méthode choisie.
    /// </exception>
    private static (HttpLabels, string) ParseRoute(string? header)
    {
        if (string.IsNullOrEmpty(header)) throw new ArgumentException("400");

        var routeParts = header.Split(' ', 3, StringSplitOptions.TrimEntries);
        if (routeParts.Length != 3) throw new ArgumentException("400");

        var method = HttpLabelsMap.GetLabel(routeParts[0]);
        var url = routeParts[1];

        if ((method == HttpLabels.Patch || method == HttpLabels.Delete || method == HttpLabels.Post ||
             method == HttpLabels.Put) && !url.Contains("/api/messages"))
            throw new ArgumentException("405");

        if (routeParts[2] != "HTTP/1.1") throw new ArgumentException("400");

        return (method, url);
    }
}