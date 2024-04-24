using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class MoveFolderHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _repository;

    private readonly MoveFolderHandler _handler;

    public MoveFolderHandler_Tests()
    {
        _repository = Substitute.For<IRepository<Folder>>();
        _handler = new MoveFolderHandler(Substitute.For<ILogger<MoveFolderHandler>>(), _repository);
    }

    public static MoveFolderCommand CreateCommand(long folderId, long newParentId)
    {
        return new Faker<MoveFolderCommand>()
                .CustomInstantiator(f => new(
                    folderId,
                    newParentId
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task MoveFolder_Success()
    {
        var mockParent = Folder_Tests.CreateFolder(null);
        var mockFolder = Folder_Tests.CreateFolder(null);
        var command = CreateCommand(mockFolder.Id, mockParent.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder, mockParent);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderForAllInfo>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveFolder_FolderNotFound()
    {
        var command = CreateCommand(1, 2);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task MoveFolder_CannotMoveFolderToItself()
    {
        var mockFolder = Folder_Tests.CreateFolder();
        var command = CreateCommand(mockFolder.Id, mockFolder.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
    }

    [Fact]
    public async Task MoveFolder_FolderIsAlreadyInParent()
    {
        var mockParent = Folder_Tests.CreateFolder(null);
        var mockFolder = Folder_Tests.CreateFolder(mockParent);
        var command = CreateCommand(mockFolder.Id, mockParent.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveFolder_NewParentFolderNotFound()
    {
        var mockFolder = Folder_Tests.CreateFolder(null);
        var command = CreateCommand(mockFolder.Id, 1);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder, (Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task MoveFolder_CannotMoveFolderToItsDescendant()
    {
        var mockFolder = Folder_Tests.CreateFolder();
        var mockSon = Folder_Tests.CreateFolder(mockFolder);
        var command = CreateCommand(mockFolder.Id, mockSon.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder, mockSon);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderForAllInfo>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
    }
}
