using shared;

namespace Server;

/// <summary>
///     Classe utilitaire pour convertir des urls.
/// </summary>
public static class UrlParser
{
    /// <summary>
    ///     Fonction utilitaire pour retourner la fonction correspondante à notre requête ainsi que l'id du message (si
    ///     nécessaire).
    /// </summary>
    /// <param name="url">Url de la requête.</param>
    /// <param name="label">Méthode de la requête.</param>
    /// <returns>Fonction correspondante et id du message.</returns>
    /// <exception cref="ArgumentException">
    ///     405 - Si url == / && méthode != GET
    ///     404 - Si url est invalide pour notre API
    ///     400 - Si méthode non supportée (GET, POST, PUT, PATCH, DELETE)
    /// </exception>
    public static (Func<string, bool, string, string> method, string param) Parse(string url, HttpLabels label)
    {
        var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if ((label == HttpLabels.Get && url.Contains("script")) || url.Contains("style")) return GetUrl(url);

        if (parts.Length == 0)
        {
            if (label != HttpLabels.Get) throw new ArgumentException("405");
            return (Views.Index, "");
        }

        if (parts.Length < 2 || parts[0] != "api" || parts[1] != "messages" || parts.Length > 3)
            throw new ArgumentException("404");

        var id = "";

        if (parts.Length == 3) id = parts[2];

        switch (label)
        {
            case HttpLabels.Get:
                return (Views.Show, id);
            case HttpLabels.Post:
                return (Views.Post, id);
            case HttpLabels.Delete:
                return (Views.Delete, id);
            case HttpLabels.Patch:
                return (Views.Update, id);
            case HttpLabels.Put:
                return (Views.Update, id);
            default:
                throw new ArgumentException("400");
        }
    }

    /// <summary>
    ///     Fonction utilitaire qui retourne nos fichiers CSS et JS (navigateur seulement)
    /// </summary>
    /// <param name="url">Url de la requête.</param>
    /// <returns>La fonction qui retourne la vue correspondante et un string vide (pas besoin de l'id)</returns>
    /// <exception cref="ArgumentException">404 - Si autre fichier demandé (non supporté)</exception>
    private static (Func<string, bool, string, string> method, string param) GetUrl(string url)
    {
        if (url.Contains("script")) return (Views.GetJs, "");

        if (url.Contains("style")) return (Views.GetCss, "");

        throw new ArgumentException("404");
    }
}