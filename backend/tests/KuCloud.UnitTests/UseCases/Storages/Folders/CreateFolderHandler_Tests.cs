using Ardalis.SharedKernel;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages.Folders;
using Microsoft.Extensions.Logging;

namespace KuCloud.UnitTests.UseCases.Storages.Folders;

public class CreateFolderHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _repos;
    private readonly CreateFolderHandler _handler;

    public CreateFolderHandler_Tests()
    {
        _repos = Substitute.For<IRepository<Folder>>();

        _handler = new(Substitute.For<ILogger<CreateFolderHandler>>(), _repos);
    }

    public static CreateFolderCommand CreateCommand(long? parentId = null)
    {
        return new Faker<CreateFolderCommand>()
                .CustomInstantiator(f => new(
                    f.Lorem.Word(),
                    parentId
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task Folder_Constructor_Success()
    {
        var command = CreateCommand();
        var mockFolder = StorageNode_Tests.CreateFolder();

        _repos.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);
        _repos.When(x => x.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>()))
            .Do(x => ((Folder) x[0]).Id = mockFolder.Id);

        var result = await _handler.Handle(command, default);

        result.Value.Should().Be(mockFolder.Id);

        await _repos.Received(1).AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
