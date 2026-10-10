using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> b)
    {
        b.ToTable("uploaded_files");
        b.HasKey(x => x.Id);
        b.Property(x => x.OriginalFileName).HasMaxLength(500).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        b.Property(x => x.StorageKey).HasMaxLength(500).IsRequired();
        b.Property(x => x.Sha256).HasMaxLength(64);
        b.Property(x => x.ProcessingStatus).HasConversion<int>();
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ProcessingStatus);

        b.HasOne(x => x.User).WithMany(u => u.Files)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FileChunkConfiguration : IEntityTypeConfiguration<FileChunk>
{
    public void Configure(EntityTypeBuilder<FileChunk> b)
    {
        b.ToTable("file_chunks");
        b.HasKey(x => x.Id);
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.EmbeddingModel).HasMaxLength(120);
        b.Property(x => x.Embedding).HasColumnType("vector(1536)");
        b.HasIndex(x => new { x.FileId, x.ChunkIndex }).IsUnique();

        b.HasOne(x => x.File).WithMany(f => f.Chunks)
            .HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);

        // IVFFlat index — created in a later migration once data exists
    }
}
