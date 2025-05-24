namespace TinyStorage.Disk;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IOPath = System.IO.Path;

internal sealed class DiskStorageContainer : StorageContainer
{
    private static readonly char[] InvalidPathChars =
        [
            IOPath.PathSeparator,
            IOPath.DirectorySeparatorChar,
            IOPath.AltDirectorySeparatorChar,
            IOPath.VolumeSeparatorChar,
        ];

    private readonly string _containerPath;

    public DiskStorageContainer(DiskStorageProvider provider, StorageContainerPath path)
        : base(provider, path)
    {
        _containerPath = GetContainerPath(provider.BasePath, path);
    }

    private static string GetContainerPath(string basePath, StorageContainerPath path)
    {
        // Basic validation: The storage container path should not contain any file system specific path separators.
        if (path.Segments.SelectMany(@char => @char).Any(c => InvalidPathChars.Contains(c)))
        {
            throw new InvalidStorageContainerPathException(
                $"The provided {nameof(StorageContainerPath)} contains one or more invalid character(s). " +
                $"The {nameof(DiskStorageProvider)} prevents using any path separator characters.");
        }

        var segmentPath = IOPath.Combine(path.Segments.ToArray());
        var containerPath = IOPath.Combine(basePath, segmentPath);

        // Sanity check:
        // This *shouldn't* throw at this point, but having it may prevent some *very* weird edge cases.
        if (!PathUtils.TryGetFullPath(containerPath, out var fullContainerPath))
        {
            throw new InvalidStorageContainerPathException(
                $"The constructed file system path of this {nameof(DiskStorageContainer)} is not a valid path. " +
                $"This exception typically occurs when the container's {nameof(Path)} property contains " +
                $"segments with invalid path characters. " +
                $"To resolve this issue, verify that the base path of the {nameof(DiskStorageProvider)} combined with " +
                $"all path segments of the {nameof(DiskStorageContainer)} result in a valid file system path.");
        }

        return fullContainerPath;
    }

    public override Task<bool> ExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = Directory.Exists(_containerPath);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(fileName);

        try
        {
            var result = File.Exists(filePath);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task CreateIfNotExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            Directory.CreateDirectory(_containerPath);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task<IEnumerable<string>> ListFilesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var fileNames = Directory.EnumerateFiles(_containerPath).Select(IOPath.GetFileName);
            return Task.FromResult(fileNames);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task<IEnumerable<string>> ListContainersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var directoryNames = Directory.EnumerateDirectories(_containerPath).Select(IOPath.GetFileName);
            return Task.FromResult(directoryNames);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task<Stream> OpenReadAsync(string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(fileName);

        try
        {
            var stream = File.OpenRead(filePath);
            return Task.FromResult<Stream>(stream);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (FileNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task<Stream> OpenWriteAsync(string fileName, bool overwrite, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(fileName);

        try
        {
            var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
            var stream = File.Open(filePath, fileMode, FileAccess.Write);
            return Task.FromResult<Stream>(stream);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (FileNotFoundException ex)
        {
            throw new StorageItemNotFoundException(null, ex);
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task DeleteAsync(CancellationToken cancellationToken)
    {
        try
        {
            Directory.Delete(_containerPath, true);
            return Task.CompletedTask;
        }
        catch (DirectoryNotFoundException)
        {
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    public override Task DeleteFileAsync(string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(fileName);

        try
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }
        catch (DirectoryNotFoundException)
        {
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new StorageException(null, ex);
        }
    }

    private string GetFilePath(string fileName)
    {
        if (fileName is null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (fileName.Any(c => InvalidPathChars.Contains(c)))
        {
            throw new ArgumentException(
                $"The file name must not contain any path separator characters.",
                nameof(fileName));
        }

        var filePath = IOPath.Combine(_containerPath, fileName);
        if (!PathUtils.TryGetFullPath(filePath, out var fullFilePath))
        {
            throw new ArgumentException(
                $"Appending the file name to the storage container's path resulted in an invalid path.",
                nameof(fileName));
        }

        return fullFilePath;
    }
}
