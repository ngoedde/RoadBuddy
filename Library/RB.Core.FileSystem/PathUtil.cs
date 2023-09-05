namespace RB.Core.FileSystem;

public static class PathUtil
{
    public static char PathSeparator = '\\';

    /// <summary>
    /// Returns the parent path to the folder or file at the given path.
    /// </summary>
    /// <param name="path">The path to determine the parent path.</param>
    /// <returns>The parent path to the given path.</returns>
    public static string GetFolderName(string? path)
    {
        if (path == null)
            return string.Empty;
        
        var fileName = GetFileName(path);

        return new string(path[..^fileName.Length]).Trim(PathSeparator);
    }

    /// <summary>
    /// Returns the file or folder name from the path.
    /// </summary>
    /// <param name="path">The path to get the file or folder name from.</param>
    /// <returns>The file name.</returns>
    public static string GetFileName(string path)
    {
        return string.IsNullOrEmpty(path) ? string.Empty : path.Split(PathSeparator).Last();
    }

    /// <summary>
    /// Ensures that the given path ends with a path separator.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string EndingPathSeparator(string path)
    {
        if (path.Last() != PathSeparator)
            return path + PathSeparator;

        return path;
    }
}