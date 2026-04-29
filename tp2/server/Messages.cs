namespace Server;

public static class Messages
{
    private static readonly List<Message> ApiMessages = new();
    private static readonly Lock Lock = new();
    private static int _nextId;

    public static Message? GetMessage(int id)
    {
        lock (Lock)
        {
            return ApiMessages.FirstOrDefault(m => m.Id == id) ?? null;
        }
    }

    public static List<Message> GetMessages()
    {
        lock (Lock)
        {
            return ApiMessages;
        }
    }

    public static void AddMessage(string message)
    {
        lock (Lock)
        {
            ApiMessages.Add(new Message(GetNextId(), message));
        }
    }

    public static void UpdateMessage(int id, string message)
    {
        lock (Lock)
        {
            ApiMessages.FirstOrDefault(m => m.Id == id)?.Content = message;
        }
    }

    public static void RemoveMessage(int id)
    {
        lock (Lock)
        {
            ApiMessages.RemoveAll(m => m.Id == id);
        }
    }

    private static int GetNextId()
    {
        return Interlocked.Increment(ref _nextId);
    }
}