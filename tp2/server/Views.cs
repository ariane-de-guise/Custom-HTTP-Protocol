using System.Text;
using System.Text.Json;

namespace Server;

public static class Views
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string Index(string _, bool html, string __)
    {
        if (html) return LoadPage("index.html");

        var message = "Welcome to the message server! Use the following endpoints to interact with the API:\n" +
                      "- GET / : Return to the home page\n" +
                      "- GET /api/messages : Retrieve all messages\n" +
                      "- GET /api/messages/{id} : Retrieve a single message\n" +
                      "- POST /api/messages : Create a new message\n" +
                      "- DELETE /api/messages/{id} : Delete a message\n" +
                      "- PUT /api/messages/{id} : Update a message\n" +
                      "- PATCH /api/messages/{id} : Partially update a message";
        return JsonSerializer.Serialize(message, Options);
    }

    public static string Show(string id, bool html, string __)
    {
        if (string.IsNullOrEmpty(id)) return GetAllMessages(html);

        return GetMessage(id, html);
    }

    public static string Post(string _, bool html, string message)
    {
        var text = message.Replace("message=", "");
        Messages.AddMessage(text);

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message created successfully.", Options);
    }

    public static string Patch(string id, bool html, string message)
    {
        var text = message.Replace("_method=PATCH&message=", "");
        Messages.UpdateMessage(int.Parse(id), text);

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message updated successfully.", Options);
    }

    public static string Delete(string id, bool html, string _)
    {
        Messages.RemoveMessage(int.Parse(id));

        if (html) return "REDIRECT";

        return JsonSerializer.Serialize("Message deleted successfully.", Options);
    }

    public static string GetCss(string _, bool __, string ___)
    {
        var path = Path.Combine("Styles", "style.css");
        return File.ReadAllText(path);
    }

    public static string GetJs(string _, bool __, string ___)
    {
        var path = Path.Combine("Scripts", "script.js");
        return File.ReadAllText(path);
    }

    private static string LoadPage(string fileName)
    {
        var path = Path.Combine("Pages", fileName);
        return File.ReadAllText(path);
    }

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
                                       <input type="hidden" name="_method" value="PATCH">
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