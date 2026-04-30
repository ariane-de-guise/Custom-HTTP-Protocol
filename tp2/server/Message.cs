namespace Server;

/// <summary>
///     Classe représentant un message dans notre API. Chaque message a un id unique et un contenu textuel.
/// </summary>
/// <param name="id">Id unique du message.</param>
/// <param name="content">Contenu du message.</param>
public class Message(int id, string content)
{
    public int Id { get; } = id;
    public string Content { get; set; } = content;
}