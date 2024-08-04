namespace SignalChat.Api.Models;

public class MessageModel
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Chat { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public virtual UserModel FromUser { get; set; } = null!;
    public virtual UserModel ToUser { get; set; } = null!;
}
