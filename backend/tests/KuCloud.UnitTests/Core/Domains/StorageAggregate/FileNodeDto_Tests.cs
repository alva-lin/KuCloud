using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class FileNodeDto_Tests : BasicTest
{
    [Fact]
    public void FileNodeDto_Create_Success()
    {
        // Arrange
        var file = FileNode_Tests.CreateFile();

        // Act
        var dto = StorageNodeDto.Create(file) as FileNodeDto;

        // Assert
        Assert.NotNull(dto);
        dto.Id.Should().Be(file.Id);
        dto.Type.Should().Be(file.Type);
        dto.Name.Should().Be(file.Name);

        dto.Attributes.Should().ContainKey("Size");
        dto.Attributes["Size"].Should().Be(file.Size);
    }
}
