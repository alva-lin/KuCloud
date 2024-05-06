// using KuCloud.Api.Contributors;
// using KuCloud.Infrastructure.Data;
//
// namespace KuCloud.FunctionalTests.ApiEndpoints;
//
// [Collection("Sequential")]
// public class ContributorList(AppFixture app) : TestBase<AppFixture>
// {
//     [Fact]
//     public async Task ReturnsTwoContributors()
//     {
//         var result = await app.Client.GetAndDeserializeAsync<ContributorListResponse>("/Contributors");
//
//         Assert.Equal(2, result.Contributors.Count);
//         Assert.Contains(result.Contributors, i => i.Name == SeedData.Contributor1.Name);
//         Assert.Contains(result.Contributors, i => i.Name == SeedData.Contributor2.Name);
//     }
// }
