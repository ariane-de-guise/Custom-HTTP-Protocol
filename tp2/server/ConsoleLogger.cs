using shared;

namespace Server;

/// <summary>
///     Classe utilitaire pour logger des informations dans la console.
/// </summary>
public static class ConsoleLogger
{
    /// <summary>
    ///     Fonction utilitaire pour logger les requêtes et réponses HTML.
    /// </summary>
    /// <param name="headers">Headers de la requête.</param>
    /// <param name="body">Body de la requête.</param>
    /// <param name="statusCode">Status code de la réponse.</param>
    /// <param name="client">Adresse IP du client.</param>
    /// <param name="response">Body de la réponse.</param>
    public static void Log(Dictionary<HttpLabels, string> headers, string body, int statusCode, string client,
        string response)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine($"Client: {client}");
        Console.WriteLine("═════════════════════════ REQUEST ═════════════════════════");
        if (headers.Count != 0)
        {
            Console.WriteLine($"Method: {Enum.GetName(headers.ElementAt(0).Key)}");
            Console.WriteLine($"Route: {headers.ElementAt(0).Value}");
        }

        if (body.Length > 0)
        {
            Console.WriteLine("Body:");
            Console.WriteLine(body);
            Console.WriteLine($"Size: {headers[HttpLabels.ContentLength]} bytes");
        }

        Console.WriteLine("═════════════════════════ RESPONSE ════════════════════════");
        Console.WriteLine($"Status:   {statusCode} ({StatusCodeHelper.GetReasonPhrase(statusCode)})");

        if (response.Length > 0)
        {
            Console.WriteLine("Body:");
            Console.WriteLine(response);
            Console.WriteLine($"Size: {response.Length} bytes");
        }

        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
    }
}