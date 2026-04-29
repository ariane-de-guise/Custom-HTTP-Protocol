namespace Server;

public class Message(int id, string content)
{
    public int Id { get; } = id;
    public string Content { get; set; } = content;
}