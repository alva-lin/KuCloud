using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UnitTests.Core.Domains.StorageAggregate;

public sealed class StorageType_Tests : BasicTest
{
    [Fact]
    public void Only_Have_Folder_And_File()
    {
        var types = StorageType.List;

        types.Should().HaveCount(2);
        types.Should().Contain(StorageType.Folder);
        types.Should().Contain(StorageType.File);
    }
}
