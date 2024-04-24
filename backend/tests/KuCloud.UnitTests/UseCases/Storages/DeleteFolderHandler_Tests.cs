using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class DeleteFolderHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _repository;
    private readonly DeleteFolderHandler _handler;

    public DeleteFolderHandler_Tests()
    {
        _repository = Substitute.For<IRepository<Folder>>();

        _handler = new DeleteFolderHandler(Substitute.For<ILogger<DeleteFolderHandler>>(), _repository, new NoOpMediator());
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
        var mockFolder = new Folder("Folder", null);
        var command = CreateCommand(mockFolder.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderForAllInfo>(), Arg.Any<CancellationToken>())
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

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderForAllInfo>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
