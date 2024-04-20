using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace KuCloud.Core.Domains.StorageAggregate;

public record StorageNodeDto
{
    public long Id { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<StorageType, int>))]
    public StorageType Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public List<StorageNodeDto>? Children { get; set; }

    public Dictionary<string, object> Attributes { get; } = new();

    public StorageNodeDto(StorageNode node, bool ignoreChildren = false)
    {
        Id = node.Id;
        Type = node.Type;

        Name = node.Name;
        Path = node.Path;

        switch (node)
        {
            case FileNode file:
                Attributes.Add("ContentType", file.ContentType);
                Attributes.Add("Size", file.Size);
                break;
            case Folder folder:
                if (!ignoreChildren)
                {
                    Children = folder.Children.Select(x => new StorageNodeDto(x, ignoreChildren: true)).ToList();
                }

                break;
        }
    }
}
