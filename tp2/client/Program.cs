using System.Net.Sockets;
using System.Text;

namespace client;

/// <summary>
///     Classe principale du client.
/// </summary>
public static class Program
{
    private static readonly CancellationTokenSource Cts = new();

    /// <summary>
    ///     Point d'entrée du programme.
    /// </summary>
    public static async Task Main()
    {
        Console.WriteLine("Bienvenue sur le service de requêtes API!");
        try
        {
            while (!Cts.Token.IsCancellationRequested) await Envoi();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Arrêt du service de requêtes API");
        }
    }

    /// <summary>
    ///     Menu principal du client.
    /// </summary>
    private static async Task Envoi()
    {
        AfficherMenu();
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) return;
        if (input.ToUpper() == "Q") await Cts.CancelAsync();
        else if (input == "0") await AfficherIndex();
        else if (input == "1") await AfficherMessages();
        else if (input == "2") await AjouterMessage();
        else if (input == "3") await MettreAJourMessage();
        else if (input == "4") await SupprimerMessage();
        else if (input == "5") await AfficherMessage();
        Console.WriteLine("\nAppuyez sur Enter pour continuer...");
        Console.ReadLine();
        Console.Clear();
    }

    /// <summary>
    ///     Fonction pour afficher les choix du menu principal.
    /// </summary>
    private static void AfficherMenu()
    {
        Console.WriteLine("Appuyez sur Q pour quitter à tout moment.");
        Console.WriteLine("Que souhaitez-vous faire ?:");
        Console.WriteLine("0. Afficher la page d'accueil de l'API");
        Console.WriteLine("1. Afficher les messages");
        Console.WriteLine("2. Ajouter un message");
        Console.WriteLine("3. Mettre à jour un message");
        Console.WriteLine("4. Supprimer un message");
        Console.WriteLine("5. Afficher un message");
    }

    /// <summary>
    ///     Choix pour afficher la page Index de l'API.
    /// </summary>
    private static async Task AfficherIndex()
    {
        Console.WriteLine("Page d'accueil de l'API :");
        var response = await SendRequest("GET", "/");
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Choix pour afficher tous les messages de l'API.
    /// </summary>
    private static async Task AfficherMessages()
    {
        Console.WriteLine("Messages:");
        var response = await SendRequest("GET", "/api/messages");
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Choix pour ajouter un message à l'API.
    /// </summary>
    private static async Task AjouterMessage()
    {
        Console.WriteLine("Entrez le message à ajouter :");
        var message = Console.ReadLine();
        if (string.IsNullOrEmpty(message)) return;
        var response = await SendRequest("POST", "/api/messages", message);
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Choix pour modifier un message de l'API.
    /// </summary>
    private static async Task MettreAJourMessage()
    {
        Console.WriteLine("Entrez l'ID du message à mettre à jour :");
        var messageId = Console.ReadLine();
        if (string.IsNullOrEmpty(messageId)) return;

        Console.WriteLine("Entrez le nouveau contenu du message :");
        var newContent = Console.ReadLine();
        if (string.IsNullOrEmpty(newContent)) return;

        var response = await SendRequest("PUT", $"/api/messages/{messageId}", newContent);
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Choix pour supprimer un message de l'API.
    /// </summary>
    private static async Task SupprimerMessage()
    {
        Console.WriteLine("Entrez l'ID du message à supprimer :");
        var messageId = Console.ReadLine();
        if (string.IsNullOrEmpty(messageId)) return;

        var response = await SendRequest("DELETE", $"/api/messages/{messageId}");
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Choix pour afficher un message précis de l'API.
    /// </summary>
    private static async Task AfficherMessage()
    {
        Console.WriteLine("Entrez l'ID du message à afficher :");
        var messageId = Console.ReadLine();
        if (string.IsNullOrEmpty(messageId)) return;

        var response = await SendRequest("GET", $"/api/messages/{messageId}");
        Console.WriteLine(response);
    }

    /// <summary>
    ///     Fonction pour envoyer et recevoir une requête HTTP valide à l'API.
    /// </summary>
    private static async Task<string> SendRequest(string method, string path, string body = "")
    {
        var client = new TcpClient();
        await client.ConnectAsync("localhost", 8088);
        var stream = client.GetStream();
        var request = $"{method} {path} HTTP/1.1\r\nHost: localhost:8088\r\nConnection: close\r\n";
        if (!string.IsNullOrEmpty(body)) request += $"Content-Length: {body.Length}\r\n";

        request += "\r\n";

        if (!string.IsNullOrEmpty(body)) request += body;

        if (!string.IsNullOrEmpty(body)) request += body;
        var requestBytes = Encoding.UTF8.GetBytes(request);
        await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

        var reader = new StreamReader(stream);
        var response = await reader.ReadToEndAsync();
        reader.Close();
        client.Close();
        return response;
    }
}