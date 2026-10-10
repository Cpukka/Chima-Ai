using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> b)
    {
        b.ToTable("conversations");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.UserId, x.DeletedAt });
        b.HasIndex(x => new { x.UserId, x.UpdatedAt });
        b.HasIndex(x => x.IsPinned);

        b.HasOne(x => x.User).WithMany(u => u.Conversations)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.LastModel).WithMany()
            .HasForeignKey(x => x.LastModelId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.ToTable("messages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.Role).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.ErrorCode).HasMaxLength(120);
        b.HasIndex(x => new { x.ConversationId, x.CreatedAt });
        b.HasIndex(x => x.Status);

        b.HasOne(x => x.Conversation).WithMany(c => c.Messages)
            .HasForeignKey(x => x.ConversationId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ParentMessage).WithMany()
            .HasForeignKey(x => x.ParentMessageId).OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Model).WithMany()
            .HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> b)
    {
        b.ToTable("message_attachments");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.MessageId);
        b.HasIndex(x => x.UploadedFileId);

        b.HasOne(x => x.Message).WithMany(m => m.Attachments)
            .HasForeignKey(x => x.MessageId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UploadedFile).WithMany()
            .HasForeignKey(x => x.UploadedFileId).OnDelete(DeleteBehavior.Restrict);
    }
}
