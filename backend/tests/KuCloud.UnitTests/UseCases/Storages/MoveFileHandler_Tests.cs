using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class MoveFileHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _folderRepository;
    private readonly IRepository<FileNode> _fileRepository;
    private readonly MoveFileHandler _handler;

    public MoveFileHandler_Tests()
    {
        _folderRepository = Substitute.For<IRepository<Folder>>();
        _fileRepository = Substitute.For<IRepository<FileNode>>();

        _handler = new(
            Substitute.For<ILogger<MoveFileHandler>>(),
            _folderRepository,
            _fileRepository
        );
    }

    public static MoveFileCommand CreateCommand(long[]? ids = null, long? parentId = null)
    {
        return new Faker<MoveFileCommand>()
                .CustomInstantiator(f => new(
                    ids ?? [ f.Random.Long() ],
                    parentId ?? f.Random.Long()
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task File_Move_Success()
    {
        var mockNewFolder = Folder_Tests.CreateFolder();
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder),
        };
        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), mockNewFolder.Id);

        _fileRepository.ListAsync(Arg.Any<MultipleFilesById>(), Arg.Any<CancellationToken>())
            .Returns(mockFiles);

        _folderRepository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);



        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();

        await _folderRepository.Received(1).UpdateAsync(mockFolder, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Move_NotFound()
    {
        var mockNewFolder = Folder_Tests.CreateFolder();
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder),
        };
        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), mockNewFolder.Id);

        _fileRepository.ListAsync(Arg.Any<MultipleFilesById>(), Arg.Any<CancellationToken>())
            .Returns([ ]);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _folderRepository.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Move_Conflict()
    {
        var mockNewFolder = Folder_Tests.CreateFolder();
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFolder2 = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder2),
        };
        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), mockNewFolder.Id);

        _fileRepository.ListAsync(Arg.Any<MultipleFilesById>(), Arg.Any<CancellationToken>())
            .Returns(mockFiles);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);

        await _folderRepository.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Move_SameFolder()
    {
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder),
        };
        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), mockFolder.Id);

        _fileRepository.ListAsync(Arg.Any<MultipleFilesById>(), Arg.Any<CancellationToken>())
            .Returns(mockFiles);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);

        await _folderRepository.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Move_FolderNotFound()
    {
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder),
        };
        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), -1L);

        _fileRepository.ListAsync(Arg.Any<MultipleFilesById>(), Arg.Any<CancellationToken>())
            .Returns(mockFiles);

        _folderRepository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _folderRepository.DidNotReceive().UpdateAsync(Arg.Any<Folder>(), Arg.Any<CancellationToken>());
    }
}
