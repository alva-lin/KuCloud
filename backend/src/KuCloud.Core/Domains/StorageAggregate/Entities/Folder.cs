using System.ComponentModel.DataAnnotations.Schema;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class Folder : StorageNode
{
    // EF Core required
    private Folder() { }

    public Folder(string name, Folder? parent) : base(StorageType.Folder, name, parent) { }

    private readonly List<StorageNode> _children = [ ];

    /// <summary>
    ///     The children of the folder, include files and folders
    /// </summary>
    public IReadOnlyList<StorageNode> Children => _children.AsReadOnly();

    private readonly List<FolderNesting> _descendantRelations = [ ];

    /// <summary>
    ///     The relations of the folder with all descendants
    /// </summary>
    public IReadOnlyList<FolderNesting> DescendantRelations => _descendantRelations.AsReadOnly();

    /// <summary>
    ///     The descendants of the folder, only include folders
    /// </summary>
    [NotMapped]
    public IReadOnlyList<StorageNode> Descendants => _descendantRelations
        .OrderBy(e => e.Depth).ThenBy(e => e.Descendant.Type.Value).ThenBy(e => e.Descendant.Id)
        .Select(e => e.Descendant)
        .ToList()
        .AsReadOnly();

    private readonly List<FolderNesting> _ancestorRelations = [ ];

    /// <summary>
    ///     The relations of the folder with all ancestors
    /// </summary>
    public IReadOnlyList<FolderNesting> AncestorRelations => _ancestorRelations.AsReadOnly();

    /// <summary>
    ///     The ancestors of the folder, only include folders
    /// </summary>
    [NotMapped]
    public IReadOnlyList<(Folder Ancestor, int Depth)> Ancestors => _ancestorRelations
        .Select(e => (e.Ancestor, e.Depth))
        .OrderBy(e => e.Depth)
        .ToList()
        .AsReadOnly();

    public bool IsAncestorOf(StorageNode node)
    {
        while (true)
        {
            if (node.Parent == null)
            {
                return false;
            }

            if (node.Parent == this)
            {
                return true;
            }

            node = node.Parent;
        }
    }

    /// <summary>
    ///     Remove the child node from the children list,
    ///     if the child is a folder, remove the descendant relation.
    /// </summary>
    /// <param name="node">The child node</param>
    public void RemoveChild(StorageNode node)
    {
        _children.Remove(node);

        if (node is Folder folder)
        {
            RemoveDescendant(folder);
        }
    }

    /// <summary>
    ///     Add the child node to the children list,
    ///     if the child is a folder, add the descendant relation.
    /// </summary>
    /// <param name="node">The child node</param>
    public void AddChild(StorageNode node)
    {
        CheckChildName(node, autoRename: true);
        _children.Add(node);

        if (node is Folder folder)
        {
            AddDescendant(folder);
        }
    }

    // TODO - 1. 添加单元测试, 2. 改名了，要怎么通知？
    private void CheckChildName(StorageNode node, bool autoRename = false)
    {
        var hasSameName = (StorageNode e) => e.Type == node.Type && e.Id != node.Id && e.Name == node.Name;

        if (_children.All(e => !hasSameName(e))) return;

        if (!autoRename) throw new InvalidOperationException("The name already exists");

        var index = 1;
        var originName = node.Name;
        while (_children.Any(hasSameName))
        {
            node.Name = $"{originName} ({index++})";
        }
    }


    /// <summary>
    ///     将给定节点及其子孙节点添加到 DescendantRelations 中，并递归至父节点，进行相同操作。
    /// </summary>
    /// <param name="descendant">子孙节点</param>
    /// <param name="depth">深度</param>
    private void AddDescendant(Folder descendant, int depth = 1)
    {
        AddDescendantInner(descendant, depth);

        Parent?.AddDescendant(descendant, depth + 1);
    }

    /// <summary>
    ///     将子孙节点添加到 DescendantRelations 中，并将递归处理子孙节点的子孙节点。
    /// </summary>
    /// <param name="descendant">子孙节点</param>
    /// <param name="depth">深度</param>
    private void AddDescendantInner(Folder descendant, int depth)
    {
        var relation = new FolderNesting()
        {
            Ancestor = this,
            Descendant = descendant,
            Depth = depth
        };

        descendant.AddAncestors(relation);
        _descendantRelations.Add(relation);

        foreach (var child in descendant.Children.OfType<Folder>())
        {
            AddDescendantInner(child, depth + 1);
        }
    }

    /// <summary>
    ///     将给定节点及其子孙节点从 DescendantRelations 中移除，并递归至父节点，进行相同操作。
    /// </summary>
    /// <param name="descendant"></param>
    private void RemoveDescendant(Folder descendant)
    {
        var relation = _descendantRelations.FirstOrDefault(e => e.Descendant == descendant);
        if (relation != null)
        {
            descendant.RemoveAncestors(relation);
            _descendantRelations.Remove(relation);
        }

        foreach (var child in descendant.Children.OfType<Folder>())
        {
            RemoveDescendant(child);
        }

        Parent?.RemoveDescendant(descendant);
    }

    private void AddAncestors(params FolderNesting[] relations)
    {
        _ancestorRelations.AddRange(relations);
        _ancestorRelations.Sort((l, r) => l.Depth - r.Depth);
    }

    private void RemoveAncestors(params FolderNesting[] relations)
    {
        foreach (var relation in relations)
        {
            _ancestorRelations.Remove(relation);
        }
    }
}
