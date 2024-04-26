using KuCloud.Core.Domains.StorageAggregate;

// ReSharper disable InconsistentNaming

namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class Folder_Tests : BasicTest
{
    public static Folder CreateFolder(Folder? parent = null)
    {
        return new Faker<Folder>()
            .CustomInstantiator(f => new Folder(f.Lorem.Word(), parent))
            .RuleFor(e => e.Id, f => f.IndexGlobal)
            .Generate();
    }

    [Fact]
    public void Folder_Constructor_Success()
    {
        var type = StorageType.Folder;
        var name = Fake.Lorem.Word();
        var parent = CreateFolder().OrNull(Fake);

        var folder = new Folder(name, parent);

        folder.Type.Should().Be(type);
        folder.Name.Should().Be(name);

        folder.Parent.Should().Be(parent);
        folder.ParentId.Should().Be(parent?.Id);
        folder.Parent?.Children.Should().Contain(folder);
    }

    [Fact]
    public void StorageNode_IsAncestorOf_Success()
    {
        var ancestor = CreateFolder();
        var parent = CreateFolder(ancestor);
        var childFolder = CreateFolder(parent);
        var childFile = FileNode_Tests.CreateFile(parent);
        var descendant = CreateFolder(childFolder);

        var otherFolder = CreateFolder();
        var otherFile = FileNode_Tests.CreateFile(otherFolder);

        ancestor.IsAncestorOf(parent).Should().BeTrue();
        ancestor.IsAncestorOf(childFolder).Should().BeTrue();
        ancestor.IsAncestorOf(childFile).Should().BeTrue();
        ancestor.IsAncestorOf(descendant).Should().BeTrue();
        ancestor.IsAncestorOf(otherFolder).Should().BeFalse();
        ancestor.IsAncestorOf(otherFile).Should().BeFalse();

        parent.IsAncestorOf(ancestor).Should().BeFalse();
        parent.IsAncestorOf(childFolder).Should().BeTrue();
        parent.IsAncestorOf(childFile).Should().BeTrue();
        parent.IsAncestorOf(descendant).Should().BeTrue();
        parent.IsAncestorOf(otherFolder).Should().BeFalse();
        parent.IsAncestorOf(otherFile).Should().BeFalse();

        childFolder.IsAncestorOf(ancestor).Should().BeFalse();
        childFolder.IsAncestorOf(parent).Should().BeFalse();
        childFolder.IsAncestorOf(childFile).Should().BeFalse();
        childFolder.IsAncestorOf(descendant).Should().BeTrue();
        childFolder.IsAncestorOf(otherFolder).Should().BeFalse();
        childFolder.IsAncestorOf(otherFile).Should().BeFalse();

        descendant.IsAncestorOf(ancestor).Should().BeFalse();
        descendant.IsAncestorOf(parent).Should().BeFalse();
        descendant.IsAncestorOf(childFolder).Should().BeFalse();
        descendant.IsAncestorOf(childFile).Should().BeFalse();
        descendant.IsAncestorOf(otherFolder).Should().BeFalse();
        descendant.IsAncestorOf(otherFile).Should().BeFalse();
    }

    [Fact]
    public void Folder_AddChild_Success()
    {
        var folder = CreateFolder();
        var child = CreateFolder();

        folder.AddChild(child);

        folder.Children.Should().Contain(child);
    }

    [Fact]
    public void Folder_RemoveChild_Success()
    {
        var folder = CreateFolder();
        var child = CreateFolder();

        folder.AddChild(child);
        folder.RemoveChild(child);

        folder.Children.Should().NotContain(child);
    }

    [Fact]
    public void Folder_AddChild_HasSameName()
    {
        var folder = CreateFolder();
        var brother = CreateFolder(folder);
        var child = CreateFolder();
        child.Name = brother.Name;

        child.SetParent(folder);

        child.Name.Should().NotBe(brother.Name);
    }

    record TestFolderTree(
        Folder Root,
        Folder Node_1,
        Folder Node_2,
        Folder Node_1_1,
        Folder Node_2_1,
        Folder Node_1_1_1,
        Folder Node_2_1_1,
        Folder Node_1_1_1_1
    );

    private static TestFolderTree BuildFolderTree()
    {
        var root = CreateFolder();

        var node_1 = CreateFolder(root);
        var node_2 = CreateFolder(root);

        var node_1_1 = CreateFolder(node_1);
        var node_2_1 = CreateFolder(node_2);

        var node_1_1_1 = CreateFolder(node_1_1);

        var node_2_1_1 = CreateFolder(node_2_1);

        var node_1_1_1_1 = CreateFolder(node_1_1_1);

        return new TestFolderTree(
            root,
            node_1,
            node_2,
            node_1_1,
            node_2_1,
            node_1_1_1,
            node_2_1_1,
            node_1_1_1_1
        );
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Root()
    {
        var (
            root,
            node_1,
            node_2,
            node_1_1,
            node_2_1,
            node_1_1_1,
            node_2_1_1,
            node_1_1_1_1
            ) = BuildFolderTree();

        root.Parent.Should().BeNull();

        root.Children.Should().HaveCount(2);
        Assert.True(root.Children.SequenceEqual([ node_1, node_2 ]));

        root.AncestorRelations.Should().BeEmpty();

        root.Ancestors.Should().BeEmpty();

        root.DescendantRelations.Should().HaveCount(7);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1 && e.Depth == 1);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_2 && e.Depth == 1);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1 && e.Depth == 2);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_2_1 && e.Depth == 2);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1 && e.Depth == 3);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_2_1_1 && e.Depth == 3);
        root.DescendantRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1_1 && e.Depth == 4);


        root.Descendants.Should().HaveCount(7);
        Assert.True(root.Descendants.SequenceEqual([
            node_1,
            node_2,
            node_1_1,
            node_2_1,
            node_1_1_1,
            node_2_1_1,
            node_1_1_1_1
        ]));
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Node_1()
    {
        var (
            root,
            node_1,
            _,
            node_1_1,
            _,
            node_1_1_1,
            _,
            node_1_1_1_1
            ) = BuildFolderTree();

        node_1.Parent.Should().Be(root);

        node_1.Children.Should().HaveCount(1);
        node_1.Children.Should().Contain(node_1_1);

        node_1.AncestorRelations.Should().HaveCount(1);
        node_1.AncestorRelations.Should().Contain(e => e.Ancestor == root && e.Descendant == node_1 && e.Depth == 1);

        node_1.Ancestors.Should().HaveCount(1);
        node_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 1);

        node_1.DescendantRelations.Should().HaveCount(3);
        node_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1 && e.Depth == 1);
        node_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1_1 && e.Depth == 2);
        node_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1_1_1 && e.Depth == 3);

        node_1.Descendants.Should().HaveCount(3);
        Assert.True(node_1.Descendants.SequenceEqual([
            node_1_1,
            node_1_1_1,
            node_1_1_1_1
        ]));
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Node_1_1()
    {
        var (
            root,
            node_1,
            _,
            node_1_1,
            _,
            node_1_1_1,
            _,
            node_1_1_1_1
            ) = BuildFolderTree();

        node_1_1.Parent.Should().Be(node_1);

        node_1_1.Children.Should().HaveCount(1);
        node_1_1.Children.Should().Contain(node_1_1_1);

        node_1_1.AncestorRelations.Should().HaveCount(2);
        node_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1 && e.Depth == 2);
        node_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1 && e.Depth == 1);

        node_1_1.Ancestors.Should().HaveCount(2);
        node_1_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 2);
        node_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1 && e.Depth == 1);

        node_1_1.DescendantRelations.Should().HaveCount(2);
        node_1_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1_1 && e.Descendant == node_1_1_1 && e.Depth == 1);
        node_1_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 2);

        node_1_1.Descendants.Should().HaveCount(2);
        Assert.True(node_1_1.Descendants.SequenceEqual([
            node_1_1_1,
            node_1_1_1_1
        ]));
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Node_1_1_1()
    {
        var (
            root,
            node_1,
            _,
            node_1_1,
            _,
            node_1_1_1,
            _,
            node_1_1_1_1
            ) = BuildFolderTree();

        node_1_1_1.Parent.Should().Be(node_1_1);

        node_1_1_1.Children.Should().HaveCount(1);
        node_1_1_1.Children.Should().Contain(node_1_1_1_1);

        node_1_1_1.AncestorRelations.Should().HaveCount(3);
        node_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1 && e.Depth == 3);
        node_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1_1 && e.Depth == 2);
        node_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1_1 && e.Descendant == node_1_1_1 && e.Depth == 1);

        node_1_1_1.Ancestors.Should().HaveCount(3);
        node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 3);
        node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1 && e.Depth == 2);
        node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1_1 && e.Depth == 1);

        node_1_1_1.DescendantRelations.Should().HaveCount(1);
        node_1_1_1.DescendantRelations.Should()
            .Contain(e => e.Ancestor == node_1_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 1);

        node_1_1_1.Descendants.Should().HaveCount(1);
        Assert.True(node_1_1_1.Descendants.SequenceEqual([
            node_1_1_1_1
        ]));
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Node_1_1_1_1()
    {
        var (
            root,
            node_1,
            _,
            node_1_1,
            _,
            node_1_1_1,
            _,
            node_1_1_1_1
            ) = BuildFolderTree();

        node_1_1_1_1.Parent.Should().Be(node_1_1_1);

        node_1_1_1_1.Children.Should().BeEmpty();

        node_1_1_1_1.AncestorRelations.Should().HaveCount(4);
        node_1_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1_1 && e.Depth == 4);
        node_1_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1 && e.Descendant == node_1_1_1_1 && e.Depth == 3);
        node_1_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 2);
        node_1_1_1_1.AncestorRelations.Should()
            .Contain(e => e.Ancestor == node_1_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 1);

        node_1_1_1_1.Ancestors.Should().HaveCount(4);
        node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 4);
        node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1 && e.Depth == 3);
        node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1_1 && e.Depth == 2);
        node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1_1_1 && e.Depth == 1);

        node_1_1_1_1.DescendantRelations.Should().BeEmpty();

        node_1_1_1_1.Descendants.Should().BeEmpty();
    }

    [Fact]
    public void Folder_Check_FolderTree_For_Change_Parent()
    {
        // Arrange
        var (
            root,
            node_1,
            node_2,
            node_1_1,
            node_2_1,
            node_1_1_1,
            _,
            node_1_1_1_1
            ) = BuildFolderTree();

        // Act
        node_1_1_1.SetParent(node_2_1);

        // Assert

        // root
        {
            root.DescendantRelations.Should().HaveCount(7);
        }

        // node_1
        {
            node_1.DescendantRelations.Should().HaveCount(1);
        }

        // node_2
        {
            node_2.DescendantRelations.Should().HaveCount(4);
        }

        // node_1_1
        {
            node_1_1.DescendantRelations.Should().BeEmpty();
        }

        // node_2_1
        {
            node_2_1.Children.Should().Contain(node_1_1_1);
            node_2_1.DescendantRelations.Should().HaveCount(3);
            node_2_1.DescendantRelations.Should()
                .Contain(e => e.Ancestor == node_2_1 && e.Descendant == node_1_1_1 && e.Depth == 1);
            node_2_1.DescendantRelations.Should()
                .Contain(e => e.Ancestor == node_2_1 && e.Descendant == node_1_1_1_1 && e.Depth == 2);

            node_2_1.Descendants.Should().HaveCount(3);
            node_2_1.Descendants.Should().Contain(node_1_1_1);
            node_2_1.Descendants.Should().Contain(node_1_1_1_1);
        }

        // node_1_1_1
        {
            node_1_1_1.Parent.Should().Be(node_2_1);

            node_1_1_1.AncestorRelations.Should().HaveCount(3);
            node_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1 && e.Depth == 3);
            node_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == node_2 && e.Descendant == node_1_1_1 && e.Depth == 2);
            node_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == node_2_1 && e.Descendant == node_1_1_1 && e.Depth == 1);

            node_1_1_1.Ancestors.Should().HaveCount(3);
            node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 3);
            node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_2 && e.Depth == 2);
            node_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_2_1 && e.Depth == 1);

            node_1_1_1.DescendantRelations.Should().HaveCount(1);
            node_1_1_1.DescendantRelations.Should().Contain(e =>
                e.Ancestor == node_1_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 1);

            node_1_1_1.Descendants.Should().HaveCount(1);
            node_1_1_1.Descendants.Should().Contain(node_1_1_1_1);
        }

        // node_1_1_1_1
        {
            node_1_1_1_1.Parent.Should().Be(node_1_1_1);

            node_1_1_1_1.AncestorRelations.Should().HaveCount(4);
            node_1_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == root && e.Descendant == node_1_1_1_1 && e.Depth == 4);
            node_1_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == node_2 && e.Descendant == node_1_1_1_1 && e.Depth == 3);
            node_1_1_1_1.AncestorRelations.Should()
                .Contain(e => e.Ancestor == node_2_1 && e.Descendant == node_1_1_1_1 && e.Depth == 2);
            node_1_1_1_1.AncestorRelations.Should().Contain(e =>
                e.Ancestor == node_1_1_1 && e.Descendant == node_1_1_1_1 && e.Depth == 1);

            node_1_1_1_1.Ancestors.Should().HaveCount(4);
            node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == root && e.Depth == 4);
            node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_2 && e.Depth == 3);
            node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_2_1 && e.Depth == 2);
            node_1_1_1_1.Ancestors.Should().Contain(e => e.Ancestor == node_1_1_1 && e.Depth == 1);

            node_1_1_1_1.DescendantRelations.Should().BeEmpty();

            node_1_1_1_1.Descendants.Should().BeEmpty();
        }
    }
}
