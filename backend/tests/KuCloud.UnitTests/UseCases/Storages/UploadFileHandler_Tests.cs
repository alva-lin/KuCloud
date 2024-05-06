using System.Text;
using KuCloud.Core.Interfaces;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class UploadFileHandler_Tests : BasicTest
{
    private readonly IFileService _fileService;
    private readonly UploadFileHandler _handler;

    public UploadFileHandler_Tests()
    {
        _fileService = Substitute.For<IFileService>();

        _handler = new(
            Substitute.For<ILogger<UploadFileHandler>>(),
            _fileService
        );
    }

    public static UploadFileCommand CreateCommand(Stream? stream = null)
    {
        const string content = "This is a test content";
        stream ??= new MemoryStream(Encoding.UTF8.GetBytes(content));

        return new UploadFileCommand(stream);
    }

    [Fact]
    public async Task File_Upload_Success()
    {
        var command = CreateCommand();
        var path = "path/to/file";

        _fileService.UploadAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(path);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(path);

        await _fileService.Received(1).UploadAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>());
    }
}