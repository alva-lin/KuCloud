namespace KuCloud.Core.Interfaces;

public interface IFileService
{
    /// <summary>
    ///     Upload file to storage
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> UploadAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    ///     Download file from storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Check if file exists
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Get file size (bytes)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetSizeAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Delete file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(string path, CancellationToken cancellationToken);
}
