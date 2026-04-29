using System.Net;
using System.Net.Sockets;
using shared;

namespace Server;

public static class Program
{
    private static readonly CancellationTokenSource Cts = new();

    public static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Loopback, 8088);
        listener.Start();
        Console.WriteLine("Server started listening on port 8088");

        try
        {
            while (!Cts.Token.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(Cts.Token);
                _ = HandleClient(client);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server stopped listening on port 8088");
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        Dictionary<HttpLabels, string> headers = new();
        var body = string.Empty;
        var html = false;

        try
        {
            var stream = client.GetStream();
            var clientIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
            var reader = new StreamReader(stream);

            try
            {
                headers = await HttpReader.ReadHttpHeaders(reader);

                if (headers.TryGetValue(HttpLabels.ContentLength, out var length))
                {
                    var contentLength = int.Parse(length);
                    body = await HttpReader.ReadBody(reader, contentLength);

                    if (body.Contains("_method")) ParseHtmlBodyRequest(headers, body);

                    Console.WriteLine("Body read");
                }

                BadRequestVerification(headers, body);

                var route = headers.First();
                var (method, param) = UrlParser.Parse(route.Value, route.Key);

                (html, var contentType) = ReturnRightContentType(headers, method);

                var message = "";

                if (method == Views.Post || method == Views.Patch) message = body;

                var response = method(param, html, message);

                if (response == "REDIRECT")
                {
                    await HttpWriter.WriteRedirectAsync(stream, "/api/messages");
                    ConsoleLogger.Log(headers, body, 303, clientIp, "");
                }
                else
                {
                    var statusCode = StatusCodeHelper.GetStatusCode(headers.ElementAt(0).Key, body.Length, html);
                    await HttpWriter.WriteResponseAsync(stream, contentType, statusCode, response);
                    ConsoleLogger.Log(headers, body, statusCode, clientIp, response);
                }
            }
            catch (ArgumentException e)
            {
                int.TryParse(e.Message, out var statusCode);
                await HttpWriter.WriteResponseAsync(stream, html ? "text/html" : "application/json",
                    statusCode == 0 ? 500 : statusCode, statusCode == 0 ? e.Message : "");
                ConsoleLogger.Log(headers, body, statusCode, clientIp, string.Empty);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }

        client.Close();
    }

    private static void BadRequestVerification(Dictionary<HttpLabels, string> headers, string body)
    {
        if (!headers.ContainsKey(HttpLabels.Host)) throw new ArgumentException("400");

        if (headers[HttpLabels.Host] != "localhost:8088") throw new ArgumentException("400");

        if ((headers.ContainsKey(HttpLabels.Post) || headers.ContainsKey(HttpLabels.Put) ||
             headers.ContainsKey(HttpLabels.Patch)) && body == "")
            throw new ArgumentException("400");
    }

    private static void ParseHtmlBodyRequest(Dictionary<HttpLabels, string> headers, string body)
    {
        var postMethod = body.Split("_method=")[1].Split('&')[0];
        var postUrl = headers[HttpLabels.Post];
        headers.Remove(HttpLabels.Post);
        headers[HttpLabelsMap.GetLabel(postMethod)] = postUrl;
    }

    private static (bool, string) ReturnRightContentType(Dictionary<HttpLabels, string> headers,
        Func<string, bool, string, string> method)
    {
        if (headers.ContainsKey(HttpLabels.Accept) && headers[HttpLabels.Accept].Contains("text/html"))
            return (true, "text/html");

        if (method == Views.GetJs) return (true, "application/javascript");

        if (method == Views.GetCss) return (true, "text/css");

        return (false, "application/json");
    }
}