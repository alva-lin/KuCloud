using KuCloud.Core.Domains.StorageAggregate;
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
            .HasColumnOrder(10)
            .HasConversion(
                v => v.Name,
                v => StorageType.FromName(v, false)
            );

        builder.Property(e => e.Name)
            .HasColumnOrder(11)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DefaultNodeNameLength);

        builder.Property(e => e.ParentId)
            .HasColumnOrder(12)
            ;
    }
}

public sealed class FolderConfiguration : BasicEntityConfiguration<Folder, long>
{
    public override void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasMany(e => e.Children)
            .WithOne(e => e.Parent)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class FileConfiguration : BasicEntityConfiguration<FileNode, long>
{
    public override void Configure(EntityTypeBuilder<FileNode> builder)
    {
        builder.Property(e => e.Path)
            .HasColumnOrder(20)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DefaultNameLength);

        builder.Property(e => e.Size)
            .HasColumnOrder(21)
            .IsRequired();
    }
}

public sealed class FolderNestingConfiguration : IEntityTypeConfiguration<FolderNesting>
{
    public void Configure(EntityTypeBuilder<FolderNesting> builder)
    {
        builder.ToTable(nameof(FolderNesting).ToSnakeCase());

        builder.HasKey(e => new { e.AncestorId, e.DescendantId });

        builder.HasOne(e => e.Ancestor)
            .WithMany(e => e.DescendantRelations)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Descendant)
            .WithMany(e => e.AncestorRelations)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Depth)
            .IsRequired();
    }
}
