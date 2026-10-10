using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class MessageAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; } = null!;
    public Guid UploadedFileId { get; set; }
    public UploadedFile UploadedFile { get; set; } = null!;
}
