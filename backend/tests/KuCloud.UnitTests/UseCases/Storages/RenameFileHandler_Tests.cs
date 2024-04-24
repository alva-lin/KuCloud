using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class RenameFileHandler_Tests : BasicTest
{
    private readonly IRepository<FileNode> _repository;
    private readonly RenameFileHandler _handler;

    public RenameFileHandler_Tests()
    {
        _repository = Substitute.For<IRepository<FileNode>>();

        _handler = new(
            Substitute.For<ILogger<RenameFileHandler>>(),
            _repository
        );
    }

    public static RenameFileCommand CreateCommand(long fileId, string? name = null)
    {
        return new Faker<RenameFileCommand>()
                .CustomInstantiator(f => new(
                    fileId,
                    name ?? f.Lorem.Word()
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task File_Rename_Success()
    {
        var mockParent = Folder_Tests.CreateFolder();
        var mockFile = FileNode_Tests.CreateFile(mockParent);
        var command = CreateCommand(mockFile.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns(mockFile);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).UpdateAsync(mockFile, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Rename_FileNotFound()
    {
        var command = CreateCommand(1);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns((FileNode?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Rename_FileNameIsTheSame()
    {
        var mockParent = Folder_Tests.CreateFolder();
        var mockFile = FileNode_Tests.CreateFile(mockParent);
        var command = CreateCommand(mockFile.Id, mockFile.Name);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns(mockFile);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>());
    }
}
