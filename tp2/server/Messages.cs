using System.Collections.Concurrent;

namespace Server;

public static class Messages
{
    private static readonly ConcurrentDictionary<int, string> ApiMessages = new();
    private static int _nextId = 0;

    public static string GetMessage(int id) => ApiMessages.GetValueOrDefault(id, $"Unknown message id: {id}");
    
    public static ConcurrentDictionary<int, string> GetMessages() => ApiMessages;
    
    public static void AddMessage(string message) => ApiMessages.TryAdd(GetNextId(),message);
    
    public static void UpdateMessage(int id, string message) => ApiMessages[id] = message;
    
    public static void RemoveMessage(int id) => ApiMessages.TryRemove(id, out _);
    
    public static void ClearMessages() => ApiMessages.Clear();
    private static int GetNextId() => Interlocked.Increment(ref _nextId);
}