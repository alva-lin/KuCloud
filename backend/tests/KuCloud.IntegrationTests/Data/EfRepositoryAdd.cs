using KuCloud.Core.ContributorAggregate;
using Xunit;

namespace KuCloud.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddsContributorAndSetsId()
    {
        var testContributorName = "testContributor";
        var testContributorStatus = ContributorStatus.NotSet;
        var repository = GetRepository();
        var contributor = new Contributor(testContributorName);

        await repository.AddAsync(contributor);

        var newContributor = (await repository.ListAsync())
            .FirstOrDefault();

        Assert.Equal(testContributorName, newContributor?.Name);
        Assert.Equal(testContributorStatus, newContributor?.Status);
        Assert.True(newContributor?.Id > 0);
    }
}