namespace SignalChat.Api.Models;

public class UserModel
{
    public UserModel()
    {
        ChatMessagesFromUsers = new HashSet<MessageModel>();
        ChatMessagesToUsers = new HashSet<MessageModel>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; }
    public virtual ICollection<MessageModel> ChatMessagesFromUsers { get; set; }
    public virtual ICollection<MessageModel> ChatMessagesToUsers { get; set; }
}
