using System.Text;
using System.Text.Json;

namespace Server;

/// <summary>
///     Classe qui retourne les vues correspondantes aux requêtes.
/// </summary>
public static class Views
{
    /// <summary>
    ///     Propriété qui set les options du JsonSerializer
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    ///     Fonction qui retourne la page d'accueil de l'API
    /// </summary>
    /// <param name="_">Discarded.</param>
    /// <param name="html">Si on accpete du HTML ou non.</param>
    /// <param name="__">Discarded.</param>
    /// <returns>Page HTML ou un message de bienvenue en JSON.</returns>
    public static string Index(string _, bool html, string __)
    {
        if (html) return LoadPage("index.html");

        var message = """
                      Welcome to the message server! Use the following endpoints to interact with the API:

                      - GET /                    : Return to the home page
                      - GET /api/messages        : Retrieve all messages
                      - GET /api/messages/{id}   : Retrieve a single message
                      - POST /api/messages       : Create a new message
                      - DELETE /api/messages/{id}: Delete a message
                      - PUT /api/messages/{id}   : Update a message
                      - PATCH /api/messages/{id} : Partially update a message
                      """;

        return message;
    }

    /// <summary>
    ///     Fonction qui retourne un ou des messages.
    /// </summary>
    /// <param name="id">Id du message ou "".</param>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <param name="__">Discarded.</param>
    /// <returns>Un, ou tous les messages.</returns>
    public static string Show(string id, bool html, string __)
    {
        if (string.IsNullOrEmpty(id)) return GetAllMessages(html);

        return GetMessage(id, html);
    }

    /// <summary>
    ///     Fonction qui ajoute un nouveau message. Si le HTML est accepté (navigateur), on redirige à la page show de tous les
    ///     messages.
    /// </summary>
    /// <param name="_">Discarded.</param>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <param name="message">Message à ajouter.</param>
    /// <returns>Message de redirection, ou un message de succès.</returns>
    public static string Post(string _, bool html, string message)
    {
        var text = message.Replace("message=", "");
        Messages.AddMessage(text);

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message created successfully.", Options);
    }

    /// <summary>
    ///     Fonction pour modifier un message existant.
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <param name="message">Le nouveau message qui remplacera l'ancien.</param>
    /// <returns>Message de redirection, ou un message de succès.</returns>
    public static string Update(string id, bool html, string message)
    {
        var text = message.Replace("_method=PUT&message=", "");
        Messages.UpdateMessage(int.Parse(id), text);

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message updated successfully.", Options);
    }

    /// <summary>
    ///     Fonction pour supprimer un message existant.
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <param name="_">Discarded.</param>
    /// <returns>Message de redirection, ou un message de succès.</returns>
    public static string Delete(string id, bool html, string _)
    {
        Messages.RemoveMessage(int.Parse(id));

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message deleted successfully.", Options);
    }

    /// <summary>
    ///     Fonction pour retourner le fichier de CSS.
    /// </summary>
    /// <param name="_">Discarded.</param>
    /// <param name="__">Discarded.</param>
    /// <param name="___">Discarded.</param>
    /// <returns>String du ficher complet de CSS.</returns>
    public static string GetCss(string _, bool __, string ___)
    {
        var path = Path.Combine("Styles", "style.css");
        return File.ReadAllText(path);
    }

    /// <summary>
    ///     Fonction pour retourner le fichier de JS.
    /// </summary>
    /// <param name="_">Discarded.</param>
    /// <param name="__">Discarded.</param>
    /// <param name="___">Discarded.</param>
    /// <returns>String du ficher complet de JS.</returns>
    public static string GetJs(string _, bool __, string ___)
    {
        var path = Path.Combine("Scripts", "script.js");
        return File.ReadAllText(path);
    }

    /// <summary>
    ///     Fonction pour lire les fichiers.
    /// </summary>
    /// <param name="fileName">Nom du fichier à lire.</param>
    /// <returns>Le contenu du fichier en string.</returns>
    private static string LoadPage(string fileName)
    {
        var path = Path.Combine("Pages", fileName);
        return File.ReadAllText(path);
    }

    /// <summary>
    ///     Fonction pour retourner tous les messages de l'API.
    /// </summary>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <returns>Page HTML en string ou JSON de tous les messages.</returns>
    private static string GetAllMessages(bool html)
    {
        var messages = Messages.GetMessages();

        if (html)
        {
            var sb = new StringBuilder();
            foreach (var message in messages)
                sb.AppendLine($"""
                               <li class="message-card">
                                   <div>
                                       <strong>#{message.Id}</strong>
                                       <p>{message.Content}</p>
                                   </div>
                                   <a class="button" href="/api/messages/{message.Id}">View</a>
                               </li>
                               """);

            return LoadPage("list.html").Replace("{{messages}}", sb.ToString());
        }

        return JsonSerializer.Serialize(messages, Options);
    }

    /// <summary>
    ///     Fonction pour retourner un message précis.
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <param name="html">Si on accepte du HTML ou non.</param>
    /// <returns>Page HTML en string ou JSON du message.</returns>
    private static string GetMessage(string id, bool html)
    {
        var message = Messages.GetMessage(int.Parse(id));

        if (message == null) throw new ArgumentException("404");

        if (html)
        {
            var htmlContent = $"""
                               <div class="message-card">
                                   <p>Message : {message.Content}</p>
                                   <p>ID : {message.Id}</p>
                                   <form id="editForm" action="/api/messages/{message.Id}" method="post">
                                       <input type="hidden" name="_method" value="PUT">
                                       <input type="text" id="message" name="message" value="{message.Content}" required>
                                       <button type="submit">Edit</button>
                                   </form>
                                   <form id="deleteForm" action="/api/messages/{message.Id}" method="post">
                                       <input type="hidden" name="_method" value="DELETE">
                                       <button type="submit">Delete</button>
                                   </form>
                               </div>
                               """;
            return LoadPage("show.html").Replace("{{message}}", htmlContent);
        }

        return JsonSerializer.Serialize(message, Options);
    }
}