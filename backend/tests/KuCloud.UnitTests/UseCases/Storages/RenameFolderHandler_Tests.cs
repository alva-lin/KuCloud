using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class RenameFolderHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _repos;
    private readonly RenameFolderHandler _handler;

    public RenameFolderHandler_Tests()
    {
        _repos = Substitute.For<IRepository<Folder>>();

        _handler = new RenameFolderHandler(Substitute.For<ILogger<RenameFolderHandler>>(), _repos);
    }

    public static RenameFolderCommand CreateCommand(long folderId)
    {
        return new Faker<RenameFolderCommand>()
                .CustomInstantiator(f => new(
                    folderId,
                    f.Lorem.Word()
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task Folder_Rename_Success()
    {
        const int folderId = 1;
        var mockParent = Folder_Tests.CreateFolder(null);
        var mockFolder = Folder_Tests.CreateFolder(mockParent);
        var command = CreateCommand(folderId);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _repos.Received(1).UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Rename_FolderNotFound()
    {
        const int folderId = 1;
        var command = CreateCommand(folderId);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Rename_FolderNameIsTheSame()
    {
        const int folderId = 1;
        var mockFolder = Folder_Tests.CreateFolder(null);
        var command = CreateCommand(folderId) with { Name = mockFolder.Name };

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();

        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Rename_FolderNameAlreadyExists()
    {
        var mockParent = Folder_Tests.CreateFolder(null);
        var mockSon = Folder_Tests.CreateFolder(mockParent);
        var mockFolder = Folder_Tests.CreateFolder(mockParent);

        var command = CreateCommand(0L) with { Name = mockSon.Name };

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder, mockParent);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
