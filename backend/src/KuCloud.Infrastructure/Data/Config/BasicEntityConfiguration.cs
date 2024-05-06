using KuCloud.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuCloud.Infrastructure.Data.Config;

public abstract class BasicEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity: BasicEntity<TId>
    where TId : struct, IEquatable<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // 默认生成的表名，包含双数
        // builder.ToTable(typeof(TEntity).Name.ToSnakeCase());

        builder.HasKey(e => e.Id);
        builder.HasQueryFilter(e => !e.AuditInfo.IsDelete);

        builder.Property(e => e.Id)
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd();

        builder.OwnsOne(e => e.AuditInfo, auditInfoBuilder =>
        {
            auditInfoBuilder.ToJson();

            auditInfoBuilder.Property(e => e.IsDelete)
                .IsRequired();

            auditInfoBuilder.Property(e => e.Creator)
                .HasMaxLength(DataSchemaConstants.DefaultNameLength)
                .IsRequired();

            auditInfoBuilder.Property(e => e.CreatorId)
                .HasMaxLength(DataSchemaConstants.DefaultNameLength)
                .IsRequired();

            auditInfoBuilder.Property(e => e.CreationTime)
                .IsRequired();

            auditInfoBuilder.Property(e => e.ModifiedTime);
        });
    }
}
