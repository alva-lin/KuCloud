using Ardalis.Result;
using Ardalis.SharedKernel;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;
using Microsoft.Extensions.Logging;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class CreateFolderHandler_Tests : BasicTest
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
    public async Task Folder_Create_Success()
    {
        var command = CreateCommand();
        var mockFolder = Folder_Tests.CreateFolder();

        _repos.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);
        _repos.When(x => x.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>()))
            .Do(x => ((Folder) x[0]).Id = mockFolder.Id);

        var result = await _handler.Handle(command, default);

        result.Value.Should().Be(mockFolder);

        await _repos.Received(1).AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Create_ParentFolderNotFound()
    {
        var command = CreateCommand(1);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repos.DidNotReceive().AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Create_ParentFolderFound()
    {
        var mockParentFolder = Folder_Tests.CreateFolder();
        var mockFolder = Folder_Tests.CreateFolder();
        var command = CreateCommand(mockParentFolder.Id);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockParentFolder);

        _repos.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        _repos.When(x => x.AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>()))
            .Do(x => ((Folder) x[0]).Id = mockFolder.Id);

        var result = await _handler.Handle(command, default);

        result.Value.Should().Be(mockFolder);

        await _repos.Received(1).AddAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Create_ParentFolderFound_FolderNameConflict()
    {
        var mockParentFolder = Folder_Tests.CreateFolder();
        var mockSonFolder = Folder_Tests.CreateFolder(mockParentFolder);

        var command = CreateCommand(mockParentFolder.Id) with { Name = mockSonFolder.Name };

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockParentFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
    }
}
