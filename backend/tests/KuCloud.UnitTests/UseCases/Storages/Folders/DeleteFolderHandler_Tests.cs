using Ardalis.SharedKernel;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;
using KuCloud.UseCases.Storages.Folders;
using Microsoft.Extensions.Logging;

namespace KuCloud.UnitTests.UseCases.Storages.Folders;

public class DeleteFolderHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _repository;
    private readonly DeleteFolderHandler _handler;

    public DeleteFolderHandler_Tests()
    {
        _repository = Substitute.For<IRepository<Folder>>();

        _handler = new DeleteFolderHandler(Substitute.For<ILogger<DeleteFolderHandler>>(), _repository);
    }

    public static DeleteFolderCommand CreateCommand(long folderId)
    {
        return new Faker<DeleteFolderCommand>()
                .CustomInstantiator(f => new(
                    folderId
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task Folder_Delete_Success()
    {
        var folderId = 1;
        var command = CreateCommand(folderId);
        var mockFolder = new Folder("Folder", null);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Folder_Delete_FolderNotFound()
    {
        var folderId = 1;
        var command = CreateCommand(folderId);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
