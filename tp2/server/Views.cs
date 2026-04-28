using System.Collections.Concurrent;
using System.Text;

namespace Server;

public static class Views
{
    public static string Index(string _, bool html, string __)
    {
        if (html)
        {
            string page = LoadPage("index.html");
            return page;
        }

        return "Bienvenue sur le serveur de messages ! Utilisez les endpoints suivants pour interagir avec l'API :\n" +
               "- GET /api/messages : Récupérer tous les messages\n" +
               "- POST /api/messages : Créer un nouveau message\n" +
               "- DELETE /api/messages/{id} : Supprimer un message\n" +
               "- PATCH /api/messages/{id} : Mettre à jour un message";
    }
    
    public static string GetMessages(string _, bool html, string __)
    {
        ConcurrentDictionary<int, string> messages = Messages.GetMessages();
        StringBuilder sb = new StringBuilder();
        
        if (html)
        {
            foreach (var message in messages)
            {
                sb.AppendLine($"""
                               <li>
                                   <p>{message.Value}</p>
                                   <form id="editForm" action="/api/messages/{message.Key}" method="post">
                                       <input type="hidden" name="_method" value="PATCH">
                                       <input type="text" id="message" name="message" value="{message.Value}" required>
                                       <button type="submit">Edit</button>
                                   </form>
                                   <form id="deleteForm" action="/api/messages/{message.Key}" method="post">
                                       <input type="hidden" name="_method" value="DELETE">
                                       <button type="submit">Delete</button>
                                   </form>
                               </li>
                               """);
            }
            
            string page = LoadPage("list.html").Replace("{{messages}}", sb.ToString());
            return page;
        }
        
        sb.Append('[');
        foreach (var message in messages)
        {
            sb.Append($"{{ \"id\": {message.Key}, \"text\": \"{message.Value}\" }}");
            sb.Append(',');
        }

        if (sb.Length > 1)
        {
            sb.Remove(sb.Length - 1, 1);
        }
            
        sb.Append(']');
        return sb.ToString();
    }
    
    public static string CreateMessage(string _, bool html, string message)
    {
        string text = message.Replace("message=", "");
        Messages.AddMessage(text);
        if (html)
        {
            return "REDIRECT";
        }
        return "Message created successfully.";
    }
    
    public static string UpdateMessage(string id, bool html, string message)
    {
        string text = message.Replace("_method=PATCH&message=", "");
        Messages.UpdateMessage(int.Parse(id), text);
        if (html)
        {
            return "REDIRECT";
        }
        return "Message deleted successfully.";
    }
    
    public static string DeleteMessage(string id, bool html, string _)
    {
        Messages.RemoveMessage(int.Parse(id));
        if (html)
        {
            return "REDIRECT";
        }
        return "Message deleted successfully.";
    }
    
    public static string GetCss(string _, bool __, string ___)
    {
        string path = Path.Combine("Styles", "style.css" );
        return File.ReadAllText(path);
    }
    
    public static string GetJs(string _, bool __, string ___)
    {
        string path = Path.Combine("Scripts", "script.js" );
        return File.ReadAllText(path);
    }
    
    private static string LoadPage(string fileName)
    {
        string path = Path.Combine("Pages", fileName);
        return File.ReadAllText(path);
    }
}