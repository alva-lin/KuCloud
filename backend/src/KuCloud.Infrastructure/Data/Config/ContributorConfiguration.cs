using KuCloud.Core.ContributorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuCloud.Infrastructure.Data.Config;

public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
{
    public void Configure(EntityTypeBuilder<Contributor> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(DataSchemaConstants.DefaultNameLength)
            .IsRequired();

        builder.OwnsOne(builder => builder.PhoneNumber).ToJson();

        builder.Property(x => x.Status)
            .HasConversion(
                x => x.Value,
                x => ContributorStatus.FromValue(x));
    }
}
