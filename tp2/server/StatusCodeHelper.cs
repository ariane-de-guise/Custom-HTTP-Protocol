using shared;

namespace Server;

public static class StatusCodeHelper
{
    public static string GetReasonPhrase(int error)
    {
        return error switch
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

    public static int GetStatusCode(HttpLabels label, int bodyLength, bool html)
    {
        if (label == HttpLabels.Get && bodyLength == 0 && !html) return 204;

        if (label == HttpLabels.Post) return 201;

        return 200;
    }
}