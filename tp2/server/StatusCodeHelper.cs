using shared;

namespace Server;

/// <summary>
///     Classe utilitaire pour associer un enum à des status codes.
/// </summary>
public static class StatusCodeHelper
{
    /// <summary>
    ///     Retourne la phrase associée à un status code HTTP donné. Si le code n'est pas reconnu, retourne "Unknown Error".
    /// </summary>
    /// <param name="code">Status code envoyé par le protocole.</param>
    /// <returns>Nom plus significatif du status code.</returns>
    public static string GetReasonPhrase(int code)
    {
        return code switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            303 => "See Other",
            400 => "Bad Request",
            404 => "Not Found",
            405 => "Method Not Allowed",
            500 => "Internal Server Error",
            _ => "Unknown Error"
        };
    }

    /// <summary>
    ///     Fonction utilitaire pour envoyer le status code correspondant à chaque méthode.
    /// </summary>
    /// <param name="label">Méthode HTTP (GET, POST...)</param>
    /// <param name="bodyLength">Longueur du body de la requête.</param>
    /// <param name="html">Si on retourne du HTML, CSS, JS</param>
    /// <returns>Le status code correspondant.</returns>
    public static int GetStatusCode(HttpLabels label, int bodyLength, bool html)
    {
        if (label == HttpLabels.Get && bodyLength == 0 && !html) return 204;

        if (label == HttpLabels.Post) return 201;

        return 200;
    }
}