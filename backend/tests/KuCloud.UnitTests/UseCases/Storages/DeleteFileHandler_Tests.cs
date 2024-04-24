using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class DeleteFileCommand_Tests : BasicTest
{
    private readonly IRepository<FileNode> _repository;
    private readonly DeleteFileHandler _handler;

    public DeleteFileCommand_Tests()
    {
        _repository = Substitute.For<IRepository<FileNode>>();

        _handler = new(
            Substitute.For<ILogger<DeleteFileHandler>>(),
            _repository
        );
    }

    public static DeleteFileCommand CreateCommand(long? fileId = null)
    {
        return new Faker<DeleteFileCommand>()
                .CustomInstantiator(f => new(
                    fileId ?? f.Random.Long()
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task File_Delete_Success()
    {
        var command = CreateCommand();
        var mockFile = FileNode_Tests.CreateFile();

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns(mockFile);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();

        await _repository.Received().DeleteAsync(mockFile, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Delete_NotFound()
    {
        var command = CreateCommand();

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns((FileNode?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
