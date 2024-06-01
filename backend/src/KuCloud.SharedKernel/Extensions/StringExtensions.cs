namespace KuCloud.SharedKernel.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Append a index to the directory path or file name to avoid duplication
    /// </summary>
    /// <example>
    ///     "path/to/file.txt".WithIndex(1) => "path/to/file (1).txt"
    ///     <br />
    ///     "path/to/folder".WithIndex(2) => "path/to/folder (2)"
    /// </example>
    /// <param name="path"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string WithIndex(this string path, int index)
    {
        var extension = Path.GetExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        return $"{name} ({index}){extension}";
    }
}
