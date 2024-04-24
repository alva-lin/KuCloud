using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Interfaces;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class DownloadFileQuery_Tests : BasicTest
{
    private readonly IReadRepository<FileNode> _repository;
    private readonly IFileService _fileService;
    private readonly DownloadFileHandler _handler;

    public DownloadFileQuery_Tests()
    {
        _repository = Substitute.For<IReadRepository<FileNode>>();
        _fileService = Substitute.For<IFileService>();

        _handler = new(
            Substitute.For<ILogger<DownloadFileHandler>>(),
            _repository,
            _fileService
        );
    }

    public static DownloadFileQuery CreateQuery(long fileId)
    {
        return new Faker<DownloadFileQuery>()
                .CustomInstantiator(f => new(
                    fileId
                ))
                .Generate()
            ;
    }

    private static Stream CreateFakeStream()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write("Hello, World!");
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    [Fact]
    public async Task File_Download_Success()
    {
        var command = CreateQuery(1);
        var mockFile = FileNode_Tests.CreateFile();

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns(mockFile);

        _fileService.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(CreateFakeStream());

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();

        await _fileService.Received(1).DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task File_Download_FileNotFound()
    {
        var command = CreateQuery(1);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFileById>(), Arg.Any<CancellationToken>())
            .Returns((FileNode?) null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _fileService.DidNotReceive().DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
