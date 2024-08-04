using Microsoft.EntityFrameworkCore;
using SignalChat.Api.Models;

namespace SignalChat.Api.Data;

public class SignalChatDbContext : DbContext
{
    public SignalChatDbContext(DbContextOptions<SignalChatDbContext> options) : base(options)
    {
    }

    public virtual DbSet<UserModel> Users { get; set; }
    public virtual DbSet<MessageModel> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MessageModel>(entity =>
        {
            entity.HasOne(message => message.FromUser)
                .WithMany(user => user.ChatMessagesFromUsers)
                .HasForeignKey(message => message.FromUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(message => message.ToUser)
                .WithMany(user => user.ChatMessagesToUsers)
                .HasForeignKey(message => message.ToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}
