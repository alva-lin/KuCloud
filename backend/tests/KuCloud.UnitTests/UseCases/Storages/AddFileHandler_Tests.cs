using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Interfaces;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class AddFileHandler_Tests : BasicTest
{
    private readonly IRepository<Folder> _folderRepos;
    private readonly IRepository<FileNode> _fileRepos;
    private readonly IFileService _fileService;
    private readonly AddFileHandler _handler;

    public AddFileHandler_Tests()
    {
        _folderRepos = Substitute.For<IRepository<Folder>>();
        _fileRepos = Substitute.For<IRepository<FileNode>>();
        _fileService = new FakeFileService();

        _handler = new(
            Substitute.For<ILogger<AddFileHandler>>(),
            _folderRepos,
            _fileRepos,
            _fileService
        );
    }

    public static AddFileCommand CreateCommand()
    {
        return new Faker<AddFileCommand>()
                .CustomInstantiator(f => new(
                    f.Random.Long(),
                    f.Lorem.Word(),
                    f.Lorem.Word()
                ))
                .Generate()
            ;
    }

    [Fact]
    public async Task File_Create_Success()
    {
        var command = CreateCommand();
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFile = FileNode_Tests.CreateFile(mockFolder);

        _folderRepos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        _fileRepos.AddAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>())
            .Returns(mockFile);
        _fileRepos.When(x => x.AddAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>()))
            .Do(x => ((FileNode) x[0]).Id = mockFile.Id);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mockFile.Id);

        await _fileRepos.Received(1).AddAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Create_FileNotFound()
    {
        var command = CreateCommand() with { Path = string.Empty };
        var mockFolder = Folder_Tests.CreateFolder();

        _folderRepos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns(mockFolder);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _fileRepos.DidNotReceive().AddAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Create_FolderNotFound()
    {
        var command = CreateCommand();
        var mockFile = FileNode_Tests.CreateFile();

        _folderRepos.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _fileRepos.DidNotReceive().AddAsync(Arg.Any<FileNode>(), Arg.Any<CancellationToken>());
    }
}
