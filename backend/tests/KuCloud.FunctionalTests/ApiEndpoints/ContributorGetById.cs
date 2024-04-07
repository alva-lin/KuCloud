using KuCloud.Api.Contributors;
using KuCloud.Infrastructure.Data;

namespace KuCloud.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorGetById(AppFixture app) : TestBase<AppFixture>
{
    [Fact]
    public async Task ReturnsSeedContributorGivenId1()
    {
        var result =
            await app.Client.GetAndDeserializeAsync<ContributorRecord>(GetContributorByIdRequest.BuildRoute(1));

        Assert.Equal(1, result.Id);
        Assert.Equal(SeedData.Contributor1.Name, result.Name);
    }

    [Fact]
    public async Task ReturnsNotFoundGivenId1000()
    {
        var route = GetContributorByIdRequest.BuildRoute(1000);
        _ = await app.Client.GetAndEnsureNotFoundAsync(route);
    }
}
