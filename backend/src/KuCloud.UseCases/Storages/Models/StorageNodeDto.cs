using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record StorageNodeDto : BasicDto
{
    public long Id { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<StorageType, int>))]
    public StorageType Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    /// <summary>
    ///     文件大小
    /// </summary>
    public long Size { get; set; }

    public static StorageNodeDto Create(StorageNode entity)
    {
        return new StorageNodeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Size = entity is FileNode file ? file.Size : 0,
            AuditRecord = entity.AuditInfo.ToRecord(),
        };
    }
}

public sealed record FolderDto : StorageNodeDto
{
    public bool IsRoot => Ancestors.Length == 0;

    public AncestorInfo[] Ancestors { get; set; } = [ ];
    public StorageNodeDto[] Children { get; set; } = [ ];

    public static FolderDto Map(Folder entity)
    {
        var ancestors = entity.Ancestors
            .Select(e => new AncestorInfo(e.Ancestor.Id, e.Ancestor.Name, entity.Ancestors.Count - (e.Depth - 1)))
            .OrderBy(e => e.Level)
            .ToArray();

        return new FolderDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Name = entity.Name,
            Ancestors = ancestors,
            Children = entity.Children.Select(StorageNodeDto.Create).ToArray(),
            AuditRecord = entity.AuditInfo.ToRecord(),
        };
    }
}

/// <summary>
///     祖先信息
/// </summary>
/// <param name="Id">祖先节点 Id</param>
/// <param name="Name">祖先名称</param>
/// <param name="Level">深度等级，根节点为1</param>
public sealed record AncestorInfo(long Id, string Name, int Level);
