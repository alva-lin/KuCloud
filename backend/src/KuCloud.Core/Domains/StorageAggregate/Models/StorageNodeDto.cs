using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace KuCloud.Core.Domains.StorageAggregate;

public abstract record StorageNodeDto
{
    public long Id { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<StorageType, int>))]
    public StorageType Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    protected StorageNodeDto(StorageNode node)
    {
        Id = node.Id;
        Type = node.Type;
        Name = node.Name;
    }

    public static StorageNodeDto Create(StorageNode node, bool ignoreChildren = false) => node switch
    {
        FileNode file => new FileNodeDto(file),
        Folder folder => new FolderDto(folder, ignoreChildren),
        _ => throw new NotSupportedException()
    };
}
