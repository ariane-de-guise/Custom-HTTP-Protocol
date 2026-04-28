using shared;

namespace Server;

public class UrlParser
{
    public static (Func<string, bool, string, string> method , string param) Parse(string url, HttpLabels label)
    {
        string[] parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine(label);
        Console.WriteLine(url);
        
        if (parts.Length == 0)
        {
            if (label != HttpLabels.Get)
            {
                throw new ArgumentException("405");
            }
            return (Views.Index, "");
        }

        if (label == HttpLabels.Get && parts[0] != "api")
        {
            return GetUrl(url);
        }

        if (parts.Length < 2 || parts[0] != "api" || parts[1] != "messages" || parts.Length > 3)
        {
            throw new ArgumentException("404");
        }
        
        string id = "";
        
        if (parts.Length == 3) id = parts[2];
        
        if (HttpLabels.Post == label)
        {
            
        }

        switch (label)
        {
            case HttpLabels.Get:
                return (Views.GetMessages, id);
            case HttpLabels.Post:
                return (Views.CreateMessage, id);
            case HttpLabels.Delete:
                return (Views.DeleteMessage, id);
            case HttpLabels.Patch:
                return (Views.UpdateMessage, id);
            default:
                throw new ArgumentException("405");
        }
    }
    
    private static (Func<string, bool, string, string> method , string param) GetUrl(string url)
    {
        if (url.Contains("script"))
        {
            return (Views.GetJs, "");
        }
        
        if (url.Contains("style"))
        {
            return (Views.GetCss, "");
        }
        
        throw new ArgumentException("404");
    }
}