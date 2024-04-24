using KuCloud.Core.Interfaces;

namespace KuCloud.Infrastructure.Services;

public class LocalStorageService : IFileService
{
    private const string BasePath = "wwwroot/files";

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private string GetFullPath(string path)
    {
        return Path.Combine(BasePath, path);
    }

    private (string FolderPath, string FileName, string FullPath) GeneratePath()
    {
        // 生成文件路径，以 yyyy-MM-dd 为文件夹，根据时间戳有序的 GUID
        var now = DateTime.Now;
        var folder = now.ToString("yyyy-MM-dd");
        var fileName = $"{now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";
        return (folder, fileName, Path.Combine(folder, fileName));
    }

    public async Task<string> UploadAsync(Stream stream, CancellationToken cancellationToken)
    {
        var (folder, fileName, path) = GeneratePath();
        EnsureDirectory(folder);

        await using var fileStream = File.Create(GetFullPath(path));
        await stream.CopyToAsync(fileStream, cancellationToken);

        return path;
    }

    public Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(path);

        Stream stream = File.OpenRead(fullPath);

        return Task.FromResult(stream);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(path);

        return await Task.FromResult(File.Exists(fullPath));
    }

    public async Task<long> GetSizeAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(path);

        var fileInfo = new FileInfo(fullPath);

        return await Task.FromResult(fileInfo.Length);
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(path);

        File.Delete(fullPath);

        return await Task.FromResult(true);
    }
}
