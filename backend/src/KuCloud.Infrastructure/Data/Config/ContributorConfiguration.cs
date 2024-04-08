using KuCloud.Core;
using KuCloud.Core.ContributorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuCloud.Infrastructure.Data.Config;

public class ContributorConfiguration : BasicEntityConfiguration<Contributor, int>
{
    public override void Configure(EntityTypeBuilder<Contributor> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
            .HasMaxLength(DataSchemaConstants.DefaultNameLength)
            .IsRequired();

        builder.OwnsOne(e => e.PhoneNumber).ToJson();
    }
}

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

        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.OwnsOne(e => e.AuditInfo, auditInfoBuilder =>
        {
            auditInfoBuilder.ToJson();

            auditInfoBuilder.Property(e => e.Creator)
                .HasMaxLength(DataSchemaConstants.DefaultNameLength)
                .IsRequired();

            auditInfoBuilder.Property(e => e.CreatorId)
                .HasMaxLength(DataSchemaConstants.DefaultNameLength)
                .IsRequired();

            auditInfoBuilder.Property(e => e.CreationTime)
                .IsRequired();

            auditInfoBuilder.Property(e => e.ModifiedTime);

            auditInfoBuilder.Property(e => e.IsDelete)
                .IsRequired();
        });

    }
}
