using System.Net.Sockets;
using shared;

namespace Server;

public class Program {
    private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
    
    public static async Task Main() {
        TcpListener listener = TcpListener.Create(8088);
        listener.Start();
        Console.WriteLine("Server started");
        
        while (!Cts.Token.IsCancellationRequested)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        try
        {
            Console.WriteLine("Client connected");
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            Console.WriteLine("Client accepted");
            bool html = false;
            try
            {
                Dictionary<HttpLabels, string> headers = await HttpReader.ReadHttpHeaders(reader);
                Console.WriteLine("Headers read");
                
                string body = "";
                if (headers.ContainsKey(HttpLabels.ContentLength))
                {
                    int contentLength = int.Parse(headers[HttpLabels.ContentLength]);
                    body = await HttpReader.ReadBody(reader, contentLength);
                    if (body.Contains("_method"))
                    {
                        string postMethod = body.Split("_method=")[1].Split('&')[0];
                        string postUrl = headers[HttpLabels.Post];
                        headers.Remove(HttpLabels.Post);
                        headers[HttpLabelsMap.GetLabel(postMethod)] = postUrl;
                    }
                    Console.WriteLine("Body read");
                }

                if (!headers.ContainsKey(HttpLabels.Host))
                {
                    throw new ArgumentException("400");
                }
                
                if (headers[HttpLabels.Host] != "localhost:8088")
                {
                    throw new ArgumentException("400");
                }
                
                if ((headers.ContainsKey(HttpLabels.Post) || headers.ContainsKey(HttpLabels.Put) || headers.ContainsKey(HttpLabels.Patch)) && body == "")
                {
                    throw new ArgumentException("204");
                }
                
                if (headers.ContainsKey(HttpLabels.Accept) && headers[HttpLabels.Accept].Contains("text/html"))
                {
                    html = true;
                }
                
                var route = headers.First();
                var (method, param) = UrlParser.Parse(route.Value, route.Key);
                
                string contentType = "application/json";
                
                if (html)
                {
                    contentType = "text/html";
                }
                else if (method == Views.GetJs)
                {
                    contentType = "application/javascript";
                }
                else if (method == Views.GetCss)
                {
                    contentType = "text/css";
                }

                string message = "";
                if (method == Views.CreateMessage || method == Views.UpdateMessage || method == Views.DeleteMessage)
                {
                    message = body;
                }
                
                string response = method(param, html, message);
                if (response == "REDIRECT")
                {
                    await HttpWriter.WriteRedirectAsync(stream, "/api/messages");
                }
                else
                {
                    await HttpWriter.WriteResponseAsync(stream, contentType, 200, response);
                }
            }
            catch (ArgumentException e)
            {
                int statusCode = int.Parse(e.Message);
                await HttpWriter.WriteResponseAsync(stream, html ? "text/html" : "application/json", statusCode);
                Console.WriteLine($"BadRequest: {e.Message}");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        
        client.Close();
        Console.WriteLine("Client disconnected");
    }
}