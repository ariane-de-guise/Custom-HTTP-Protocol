namespace shared;

/// <summary>
///     Enum contenant plusieurs en-têtes HTTP.
/// </summary>
public enum HttpLabels
{
    // Verbs
    Get,
    Put,
    Post,
    Delete,
    Patch,

    // General Headers
    CacheControl,
    Connection,
    Date,
    TransferEncoding,
    Via,

    // Request Headers
    Host,
    UserAgent,
    Accept,
    AcceptLanguage,
    AcceptEncoding,
    Authorization,
    Cookie,
    Referer,
    Origin,

    // Response Headers
    Server,
    SetCookie,
    Location,

    // Entity Headers
    ContentLength,
    ContentType,
    ContentLanguage,
    ContentEncoding,

    // Misc
    Unknown
}