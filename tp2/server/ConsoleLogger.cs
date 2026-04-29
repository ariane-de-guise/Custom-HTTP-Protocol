using shared;

namespace Server;

public static class ConsoleLogger
{
    public static void Log(Dictionary<HttpLabels, string> headers, string body, int statusCode, string client,
        string response)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine($"Client: {client}");
        Console.WriteLine("═════════════════════════ REQUEST ═════════════════════════");
        Console.WriteLine($"Method: {Enum.GetName(headers.ElementAt(0).Key)}");
        Console.WriteLine($"Route: {headers.ElementAt(0).Value}");

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