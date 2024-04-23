using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class FileNode_Tests : BasicTest
{
    public static FileNode CreateFile(Folder? parent = null)
    {
        return new Faker<FileNode>()
            .CustomInstantiator(f => new FileNode(parent, f.Lorem.Word(), f.System.MimeType(), f.Random.Long()))
            .RuleFor(e => e.Id, f => f.IndexGlobal)
            .Generate();
    }

    [Fact]
    public void Constructor_Success()
    {
        var type = StorageType.File;
        var name = Fake.Lorem.Word();
        var parent = Folder_Tests.CreateFolder().OrNull(Fake);
        var contentType = Fake.System.MimeType();
        var size = Fake.Random.Long();

        var file = new FileNode(parent, name, contentType, size);

        file.Type.Should().Be(type);
        file.Name.Should().Be(name);
        file.ContentType.Should().Be(contentType);
        file.Size.Should().Be(size);

        file.Parent.Should().Be(parent);
        file.ParentId.Should().Be(parent?.Id);
        file.Parent?.Children.Should().Contain(file);
    }
}
