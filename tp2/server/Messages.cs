namespace Server;

/// <summary>
///     Classe qui garde en mémoire tous les messages de l'API.
/// </summary>
public static class Messages
{
    private static readonly List<Message> ApiMessages = new();
    private static readonly Lock Lock = new();
    private static int _nextId;

    /// <summary>
    ///     Fonction pour recevoir un seul message par son id. Si le message n'existe pas, une exception est levée avec le
    ///     message "404".
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <returns>Le message recherché.</returns>
    /// <exception cref="ArgumentException">Si le message n'existe pas.</exception>
    public static Message GetMessage(int id)
    {
        lock (Lock)
        {
            return ApiMessages.FirstOrDefault(m => m.Id == id) ?? throw new ArgumentException("404");
        }
    }

    /// <summary>
    ///     Fonction pour retourner tous les messages de l'API.
    /// </summary>
    /// <returns>Liste de messages.</returns>
    public static List<Message> GetMessages()
    {
        lock (Lock)
        {
            return ApiMessages;
        }
    }

    /// <summary>
    ///     Fonction pour ajouter un nouveau message. L'id est défini automatiquement en incrémentant le dernier id utilisé.
    /// </summary>
    /// <param name="message">Message à ajouter.</param>
    public static void AddMessage(string message)
    {
        lock (Lock)
        {
            ApiMessages.Add(new Message(GetNextId(), message));
        }
    }

    /// <summary>
    ///     Fonction pour modifier un message existant.
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <param name="message">Nouveau message avec lequel on remplacera l'ancien.</param>
    /// <exception cref="ArgumentException">Si le message n'existe pas.</exception>
    public static void UpdateMessage(int id, string message)
    {
        lock (Lock)
        {
            if (ApiMessages.All(m => m.Id != id)) throw new ArgumentException("404");
            ApiMessages.FirstOrDefault(m => m.Id == id)?.Content = message;
        }
    }

    /// <summary>
    ///     Fonction pour supprimer un message existant.
    /// </summary>
    /// <param name="id">Id du message.</param>
    /// <exception cref="ArgumentException">Si le message n'existe pas.</exception>
    public static void RemoveMessage(int id)
    {
        lock (Lock)
        {
            if (ApiMessages.All(m => m.Id != id)) throw new ArgumentException("404");
            ApiMessages.RemoveAll(m => m.Id == id);
        }
    }

    /// <summary>
    ///     Fonction qui incrémente de manière thread safe le dernier id utilisé et retourne la nouvelle valeur.
    /// </summary>
    /// <returns>Le prochain id à utiliser.</returns>
    private static int GetNextId()
    {
        return Interlocked.Increment(ref _nextId);
    }
}