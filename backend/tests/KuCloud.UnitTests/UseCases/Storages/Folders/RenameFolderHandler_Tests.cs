using Ardalis.Result;
using Ardalis.SharedKernel;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;
using KuCloud.UseCases.Storages.Folders;
using Microsoft.Extensions.Logging;

namespace KuCloud.UnitTests.UseCases.Storages.Folders;

public class RenameFolderHandler_Tests : BasicTest
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
        var folderId = 1;
        var command = CreateCommand(folderId);
        var mockFolder = new Folder("Folder", null);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _repos.Received(1).UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Rename_FolderNotFound()
    {
        var folderId = 1;
        var command = CreateCommand(folderId);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
