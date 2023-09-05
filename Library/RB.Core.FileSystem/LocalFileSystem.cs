using RB.Core.FileSystem.IO;
using RB.Core.FileSystem.Local;
using RB.Core.FileSystem.Local.IO;

namespace RB.Core.FileSystem;

/// <inheritdoc />
public class LocalFileSystem : IFileSystem
{
    public LocalFileSystem(string basePath, bool readOnly)
    {
        BasePath = basePath;
        ReadOnly = readOnly;

        Root = new LocalFolder("", this);
    }
    

    public bool ReadOnly { get; }

    /// <summary>
    ///     Gets the root folder of the local file system.
    /// </summary>
    public IFolder Root { get; }

    /// <summary>
    ///     Gets the base path for the local file system.
    /// </summary>
    public string BasePath { get; }

    /// <summary>
    ///     Gets the path separator for the local file system.
    /// </summary>
    public char PathSeparator => Path.DirectorySeparatorChar;

    public bool FileExists(string path)
    {
        var absolutePath = GetAbsolutePath(path);

        return File.Exists(absolutePath);
    }

    public bool FolderExists(string path)
    {
        var absolutePath = GetAbsolutePath(path);

        return Directory.Exists(absolutePath);
    }

    public IFile GetFile(string path)
    {
        AssertFileExists(path);

        return new LocalFile(path, this);
    }

    public IFolder GetFolder(string path)
    {
        AssertFolderExists(path);

        return new LocalFolder(path, this);
    }

    public IEnumerable<IFile> GetFiles(string folderPath)
    {
        AssertFolderExists(folderPath);

        var absolutePath = GetAbsolutePath(folderPath);

        var fileNames = Directory.GetFiles(absolutePath);
        var result = new List<IFile>(fileNames.Length);
        result.AddRange(fileNames.Select(GetRelativePath).Select(relativePath => new LocalFile(relativePath, this)));

        return result.ToArray();
    }

    public IFolder[] GetFolders(string folderPath)
    {
        AssertFolderExists(folderPath);

        var absolutePath = GetAbsolutePath(folderPath);

        var folderNames = Directory.GetDirectories(absolutePath);
        var result = new List<IFolder>(folderNames.Length);
        result.AddRange(folderNames.Select(GetRelativePath)
            .Select(relativePath => new LocalFolder(relativePath, this)));

        return result.ToArray();
    }

    public string[] GetChildren(string folderPath)
    {
        AssertFolderExists(folderPath);

        var absolutePath = GetAbsolutePath(folderPath);

        var childrenNames = Directory.GetFileSystemEntries(absolutePath);
        var result = new string[childrenNames.Length];

        for (var iChild = 0; iChild < childrenNames.Length; iChild++)
            result[iChild] = GetRelativePath(childrenNames[iChild]);

        return result;
    }

    public IFile CreateFile(string path, byte[] data)
    {
        AssertWritable();
        AssertFileNotExists(path);

        var absolutePath = GetAbsolutePath(path);
        File.WriteAllBytes(absolutePath, data);

        return GetFile(path);
    }

    public IFile CreateFile(string path, string content)
    {
        AssertWritable();
        AssertFileNotExists(path);

        var absolutePath = GetAbsolutePath(path);
        File.WriteAllText(absolutePath, content);

        return GetFile(path);
    }

    public IFolder CreateFolder(string path)
    {
        AssertWritable();
        AssertFolderNotExists(path);

        var absolutePath = GetAbsolutePath(path);
        Directory.CreateDirectory(absolutePath);

        return GetFolder(path);
    }

    public void Move(string path, string newPath)
    {
        AssertWritable();
        AssertFolderExists(path);
        AssertFolderNotExists(newPath);

        var absoluteSourcePath = GetAbsolutePath(path);
        var absoluteDestinationPath = GetAbsolutePath(newPath);

        Directory.Move(absoluteSourcePath, absoluteDestinationPath);
    }

    public void Delete(string path)
    {
        AssertWritable();

        var absolutePath = GetAbsolutePath(path);

        if (FileExists(path))
            File.Delete(absolutePath);
        else if (FolderExists(path))
            Directory.Delete(path, true);
    }

    public IFile CopyFile(string path, string destinationPath)
    {
        AssertWritable();
        AssertFileExists(path);
        AssertFileNotExists(destinationPath);

        return CreateFile(destinationPath, OpenRead(path).ReadAllBytes());
    }

    public IFolder CopyFolder(string path, string destinationPath, bool recursive = true)
    {
        AssertWritable();
        AssertFolderExists(path);
        AssertFolderNotExists(destinationPath);

        var absoluteSourcePath = GetAbsolutePath(path);
        var absoluteDestinationPath = GetAbsolutePath(destinationPath);

        CopyDirectory(absoluteSourcePath, absoluteDestinationPath, recursive);

        return GetFolder(path);
    }

    public IFileWriter OpenWrite(string path)
    {
        AssertWritable();
        AssertFileExists(path);

        var absolutePath = GetAbsolutePath(path);
        return new LocalFileWriter(File.OpenWrite(absolutePath));
    }

    public IFileReader OpenRead(string path)
    {
        AssertFileExists(path);

        var absolutePath = GetAbsolutePath(path);

        return new LocalFileReader(File.OpenRead(absolutePath));
    }

    private string GetAbsolutePath(string path)
    {
        if (!path.StartsWith(PathSeparator))
            path = PathSeparator + path;

        if (path.Contains('/') && PathSeparator != '/')
            path = path.Replace('/', PathSeparator);

        if (path.Contains('\\') && PathSeparator != '\\')
            path = path.Replace('\\', PathSeparator);

        return BasePath + path;
    }

    private string GetRelativePath(string absolutePath)
    {
        if (!absolutePath.StartsWith(BasePath))
            return absolutePath;

        var relativePathOffset = BasePath.Length + 1;
        var relativePathLength = absolutePath.Length - BasePath.Length - 1;

        var relativePath = absolutePath.Substring(relativePathOffset, relativePathLength);

        return relativePath;
    }

    private void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        if (ReadOnly)
            throw new IOException("The file system is read-only.");

        // Get information about the source folder
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source folder exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source folder not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination folder
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source folder and copy to the destination folder
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (!recursive)
            return;

        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }

    private void AssertFolderExists(string path)
    {
        if (!FolderExists(path))
            throw new DirectoryNotFoundException($"The folder {path} does not exist.");
    }

    private void AssertFileExists(string path)
    {
        if (!FileExists(path))
            throw new FileNotFoundException($"The file {path} does not exist.");
    }

    private void AssertFileNotExists(string path)
    {
        if (FileExists(path))
            throw new IOException($"The file {path} does already exist.");
    }

    private void AssertFolderNotExists(string path)
    {
        if (FolderExists(path))
            throw new IOException($"The folder {path} does already exist.");
    }

    private void AssertWritable()
    {
        if (ReadOnly)
            throw new IOException("The file system is read-only");
    }
}