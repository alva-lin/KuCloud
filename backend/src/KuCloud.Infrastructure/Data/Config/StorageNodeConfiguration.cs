using KuCloud.Core.StorageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuCloud.Infrastructure.Data.Config;

public sealed class StorageNodeConfiguration : BasicEntityConfiguration<StorageNode, long>
{
    public override void Configure(EntityTypeBuilder<StorageNode> builder)
    {
        base.Configure(builder);

        builder.UseTphMappingStrategy();

        builder.HasDiscriminator(e => e.Type)
            .HasValue<Folder>(StorageType.Folder)
            .HasValue<FileNode>(StorageType.File);

        builder.Property(e => e.Type)
            .HasConversion(
                v => v.Name,
                v => StorageType.FromName(v, false)
            );

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DefaultNodeNameLength);
    }
}

public sealed class FolderConfiguration : BasicEntityConfiguration<Folder, long>
{
    public override void Configure(EntityTypeBuilder<Folder> builder)
    {
        // base.Configure(builder);

        builder.HasMany(e => e.Children)
            .WithOne(e => e.Parent)
            .IsRequired(false);
    }
}

public sealed class FileConfiguration : BasicEntityConfiguration<FileNode, long>
{
    public override void Configure(EntityTypeBuilder<FileNode> builder)
    {
        // base.Configure(builder);

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DefaultNameLength);

        builder.Property(e => e.Size)
            .IsRequired();
    }
}
