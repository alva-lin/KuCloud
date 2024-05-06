using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddFolderAndSetsId()
    {
        var repository = GetRepository();
        var folder = new Folder("testFolder", null);

        await repository.AddAsync(folder);

        var newFolder = (await repository.ListAsync())
            .FirstOrDefault();

        Assert.Equal(folder.Name, newFolder?.Name);
        Assert.True(newFolder?.Id > 0);
    }
}
