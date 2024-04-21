using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class FolderDto_Tests : BasicTest
{
    [Fact]
    public void FolderDto_Create_Success()
    {
        // Arrange
        var folder = Folder_Tests.CreateFolder();

        // Act
        var dto = StorageNodeDto.Create(folder) as FolderDto;

        // Assert
        Assert.NotNull(dto);
        dto.Id.Should().Be(folder.Id);
        dto.Type.Should().Be(folder.Type);
        dto.Name.Should().Be(folder.Name);

        dto.Children.Should().BeEmpty();
    }

    [Fact]
    public void FolderDto_Create_WithChildren_Success()
    {
        // Arrange
        var folder = Folder_Tests.CreateFolder();
        var child = FileNode_Tests.CreateFile(folder);

        // Act
        var dto = StorageNodeDto.Create(folder) as FolderDto;

        // Assert
        Assert.NotNull(dto);

        dto.Children.Should().HaveCount(1);
        dto.Children.Should().Contain(e => e.Id == child.Id);
    }

    [Fact]
    public void FolderDto_Create_WithChildren_IgnoreChildren_Success()
    {
        // Arrange
        var folder = Folder_Tests.CreateFolder();
        _ = FileNode_Tests.CreateFile(folder);

        // Act
        var dto = StorageNodeDto.Create(folder, ignoreChildren: true) as FolderDto;

        // Assert
        Assert.NotNull(dto);

        dto.Children.Should().BeNull();
    }

    [Fact]
    public void FolderDto_Create_WithAncestors()
    {
        // Arrange
        var ancestor = Folder_Tests.CreateFolder();
        var parent = Folder_Tests.CreateFolder(ancestor);
        var folder = Folder_Tests.CreateFolder(parent);

        // Act
        var dto = StorageNodeDto.Create(folder) as FolderDto;

        // Assert
        Assert.NotNull(dto);

        dto.Ancestors.Should().HaveCount(2);
        dto.Ancestors.Should().Contain(e => e.Ancestor.Id == parent.Id && e.Depth == 1);
        dto.Ancestors.Should().Contain(e => e.Ancestor.Id == ancestor.Id && e.Depth == 2);
    }
}
