using KuCloud.Core.StorageAggregate;

namespace KuCloud.UnitTests.Core.StorageAggregate;

public sealed class StorageNode_Tests : BasicTest
{
    public static Folder CreateFolder(Folder? parent = null)
    {
        return new Faker<Folder>()
            .CustomInstantiator(f => new Folder(f.Lorem.Word(), parent))
            .Generate();
    }

    public static FileNode CreateFile(Folder? parent = null)
    {
        return new Faker<FileNode>()
            .CustomInstantiator(f => new FileNode(f.Lorem.Word(), parent, f.System.MimeType(), f.Random.Long()))
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

        folder.Path.Should().Be(parent?.Path + "/" + name);
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
    public void FileNode_Constructor_Success()
    {
        var type = StorageType.File;
        var name = Fake.Lorem.Word();
        var parent = CreateFolder().OrNull(Fake);
        var contentType = Fake.System.MimeType();
        var size = Fake.Random.Long();

        var file = new FileNode(name, parent, contentType, size);

        file.Type.Should().Be(type);
        file.Name.Should().Be(name);
        file.ContentType.Should().Be(contentType);
        file.Size.Should().Be(size);

        file.Parent.Should().Be(parent);
        file.ParentId.Should().Be(parent?.Id);
        file.Parent?.Children.Should().Contain(file);

        file.Path.Should().Be(parent?.Path + "/" + name);
    }
}
