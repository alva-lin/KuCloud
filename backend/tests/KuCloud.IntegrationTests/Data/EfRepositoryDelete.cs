// using KuCloud.Core.ContributorAggregate;
// using Xunit;
//
// namespace KuCloud.IntegrationTests.Data;
//
// public class EfRepositoryDelete : BaseEfRepoTestFixture
// {
//     [Fact]
//     public async Task DeletesItemAfterAddingIt()
//     {
//         // add a Contributor
//         var repository = GetRepository();
//         var initialName = Guid.NewGuid().ToString();
//         var contributor = new Contributor(initialName);
//         await repository.AddAsync(contributor);
//
//         // delete the item
//         await repository.DeleteAsync(contributor);
//
//         // verify it's no longer there
//         Assert.DoesNotContain(await repository.ListAsync(),
//             contributor => contributor.Name == initialName);
//     }
// }
