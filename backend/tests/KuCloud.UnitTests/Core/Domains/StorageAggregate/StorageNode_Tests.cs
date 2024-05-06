namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class StorageNode_Tests : BasicTest
{
    [Fact]
    public void SetParent_Success()
    {
        var parent = Folder_Tests.CreateFolder();
        var child = FileNode_Tests.CreateFile();

        child.SetParent(parent);

        child.Parent.Should().Be(parent);
        child.ParentId.Should().Be(parent.Id);
        parent.Children.Should().Contain(child);
    }

    [Fact]
    public void SetParent_SameParent_Success()
    {
        var parent = Folder_Tests.CreateFolder();
        var child = FileNode_Tests.CreateFile(parent);

        child.SetParent(parent);

        child.Parent.Should().Be(parent);
        child.ParentId.Should().Be(parent.Id);
        parent.Children.Should().Contain(child);
    }

    [Fact]
    public void SetParent_WhenParentIsSelf_ThrowException()
    {
        var folder = Folder_Tests.CreateFolder();

        var act = () => folder.SetParent(folder);

        act.Should().Throw<InvalidOperationException>().WithMessage("Cannot set parent to self");
    }

    [Fact]
    public void SetParent_WhenParentIsAncestor_ThrowException()
    {
        var parent = Folder_Tests.CreateFolder();
        var child = Folder_Tests.CreateFolder(parent);

        var act = () => parent.SetParent(child);

        act.Should().Throw<InvalidOperationException>().WithMessage("Cannot set parent to a descendant");
    }
}
