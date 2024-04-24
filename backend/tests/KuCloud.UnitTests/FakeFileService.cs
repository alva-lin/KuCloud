using KuCloud.Core.Interfaces;

namespace KuCloud.UnitTests;

public class FakeFileService : IFileService
{
    public Task<string> UploadAsync(Stream stream, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

    public Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken)
    {
        const string content = "This is a fake file content";

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        return Task.FromResult<Stream>(stream);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(path));
    }

    public Task<long> GetSizeAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(1024L);
    }

    public Task<bool> DeleteAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}
