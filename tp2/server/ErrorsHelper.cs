namespace Server;

public static class ErrorsHelper
{
    public enum Errors
    {
        Ok = 200,
        NoContent = 204,
        BadRequest = 400,
        NotFound = 404,
        MethodNotAllowed = 405,
        InternalServerError = 500,
    }

    public static string GetReasonPhrase(int error)
    {
        return error switch
        {
            200 => "OK",
            204 => "No Content",
            400 => "Bad Request",
            404 => "Not Found",
            405 => "Method Not Allowed",
            500 => "Internal Server Error",
            _ => "Unknown Error"
        };
    }
}

