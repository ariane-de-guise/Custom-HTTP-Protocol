using System.Net;
using System.Net.Sockets;
using shared;

namespace Server;

/// <summary>
///     Classe principale qui démarre le serveur HTTP et gère les connexions entrantes.
/// </summary>
public static class Program
{
    private static readonly CancellationTokenSource Cts = new();

    /// <summary>
    ///     Fonction principale du serveur.
    ///     Démarre l'écoute sur le port 8088 et accepte les clients.
    /// </summary>
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

    /// <summary>
    ///     Fonction utilitaire qui traite une requête HTTP complète d'un client.
    ///     Lit les headers et le body, valide la requête,
    ///     appelle la bonne route puis envoie la réponse.
    /// </summary>
    /// <param name="client">Le client TCP connecté.</param>
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
                }

                BadRequestVerification(headers, body);

                var route = headers.First();
                var (method, param) = UrlParser.Parse(route.Value, route.Key);

                (html, var contentType) = ReturnRightContentType(headers, method);

                var message = "";

                if (method == Views.Post || method == Views.Update) message = body;

                var response = method(param, html, message);

                if (response == "REDIRECT")
                {
                    await HttpWriter.WriteRedirectAsync(stream, "/api/messages");
                    ConsoleLogger.Log(headers, body, 303, clientIp, "");
                }
                else
                {
                    var statusCode = StatusCodeHelper.GetStatusCode(headers.ElementAt(0).Key, response.Length, html);
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
            await HttpWriter.WriteResponseAsync(client.GetStream(), html ? "text/html" : "application/json", 500,
                e.Message);
            Console.WriteLine($"SocketException: {e}");
        }

        client.Close();
    }

    /// <summary>
    ///     Vérifie que la requête respecte les règles minimales.
    ///     Lance une exception 400 si le Host est absent/invalide
    ///     ou si le body est vide pour POST, PUT ou PATCH.
    /// </summary>
    /// <param name="headers">Dictionnaire des headers HTTP de la requête.</param>
    /// <param name="body">Le corps de la requête.</param>
    /// <exception cref="ArgumentException">400 si la requête est invalide.</exception>
    private static void BadRequestVerification(Dictionary<HttpLabels, string> headers, string body)
    {
        if (!headers.ContainsKey(HttpLabels.Host)) throw new ArgumentException("400");

        if (headers[HttpLabels.Host] != "localhost:8088") throw new ArgumentException("400");

        if ((headers.ContainsKey(HttpLabels.Post) || headers.ContainsKey(HttpLabels.Put) ||
             headers.ContainsKey(HttpLabels.Patch)) && body == "")
            throw new ArgumentException("400");
    }

    /// <summary>
    ///     Fonction utilitaire pour parser le paramètre _method
    ///     dans les formulaires HTML envoyés en POST.
    ///     Permet de remplacer POST par la vraie méthode HTTP voulue.
    /// </summary>
    /// <param name="headers">Dictionnaire des headers HTTP à mettre à jour.</param>
    /// <param name="body">Le corps de la requête contenant le paramètre _method.</param>
    private static void ParseHtmlBodyRequest(Dictionary<HttpLabels, string> headers, string body)
    {
        var postMethod = body.Split("_method=")[1].Split('&')[0];
        var postUrl = headers[HttpLabels.Post];
        headers.Remove(HttpLabels.Post);
        headers[HttpLabelsMap.GetLabel(postMethod)] = postUrl;
    }

    /// <summary>
    ///     Fonction utilitaire pour choisir le Content-Type de la réponse
    ///     selon le header Accept et la vue appelée.
    /// </summary>
    /// <param name="headers">Dictionnaire des headers HTTP de la requête.</param>
    /// <param name="method">Méthode de vue appelée.</param>
    /// <returns>
    ///     Tuple contenant
    ///     - un booléen (true si la réponse est traitée comme page/asset web)
    ///     - le type de contenu de la réponse (text/html, application/javascript, text/css ou application/json)
    /// </returns>
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