using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.IntegrationTests.Data;

public class EfRepositoryDelete : BaseEfRepoTestFixture
{
    [Fact]
    public async Task DeletesItemAfterAddingIt()
    {
        // add a Contributor
        var repository = GetRepository();
        var initialName = Guid.NewGuid().ToString();
        var contributor = new Folder(initialName, null);
        await repository.AddAsync(contributor);

        // delete the item
        await repository.DeleteAsync(contributor);

        // verify it's no longer there
        Assert.DoesNotContain(await repository.ListAsync(),
            e => e.Name == initialName);
    }
}
