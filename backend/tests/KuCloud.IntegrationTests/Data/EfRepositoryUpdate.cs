using KuCloud.Core.Domains.StorageAggregate;
using Microsoft.EntityFrameworkCore;

namespace KuCloud.IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
    [Fact]
    public async Task UpdateFolderAfterAddingIt()
    {
        // add a Folder
        var repository = GetRepository();
        var initialName = Guid.NewGuid().ToString();
        var folder = new Folder(initialName, null);

        await repository.AddAsync(folder);

        // detach the item so we get a different instance
        DbContext.Entry(folder).State = EntityState.Detached;

        // fetch the item and update its title
        var newFolder = (await repository.ListAsync())
            .FirstOrDefault(e => e.Name == initialName);
        if (newFolder == null)
        {
            Assert.NotNull(newFolder);
            return;
        }

        Assert.NotSame(folder, newFolder);
        var newName = Guid.NewGuid().ToString();
        newFolder.Name = newName;

        // Update the item
        await repository.UpdateAsync(newFolder);

        // Fetch the updated item
        var updatedItem = (await repository.ListAsync())
            .FirstOrDefault(folder => folder.Name == newName);

        Assert.NotNull(updatedItem);
        Assert.NotEqual(folder.Name, updatedItem.Name);
        Assert.Equal(newFolder.Id, updatedItem.Id);
    }
}
