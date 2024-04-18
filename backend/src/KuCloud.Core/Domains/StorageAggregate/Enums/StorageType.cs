using Ardalis.SmartEnum;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class StorageType(string name, int value) : SmartEnum<StorageType>(name, value)
{
    // EF Core required
    // ReSharper disable once UnusedMember.Local
    public StorageType() : this("Unknown", 0) { }

    public static readonly StorageType Folder = new(nameof(Folder), 1);

    public static readonly StorageType File = new(nameof(File), 2);
}
