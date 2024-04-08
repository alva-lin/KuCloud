using Ardalis.SharedKernel;
using KuCloud.Core.ContributorAggregate;
using KuCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KuCloud.IntegrationTests.Data;

public abstract class BaseEfRepoTestFixture
{
    protected AppDbContext DbContext;

    protected BaseEfRepoTestFixture()
    {
        var options = CreateNewContextOptions();
        var fakeLogger = Substitute.For<ILogger<AppDbContext>>();
        var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();

        DbContext = new AppDbContext(options, fakeLogger, fakeEventDispatcher);
    }

    protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase("KuCloud.IntegrationTests")
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    protected EfRepository<Contributor> GetRepository()
    {
        return new EfRepository<Contributor>(DbContext);
    }
}
