namespace TinyStorage.Disk;

using System;

/// <summary>
/// A <see cref="StorageProvider"/> which stores data on the local file system.
/// </summary>
public sealed class DiskStorageProvider : StorageProvider
{
    /// <summary>
    /// The path to the directory where all data should be stored.
    /// </summary>
    public string BasePath { get; }

    /// <summary>
    /// Initializes a new <see cref="DiskStorageProvider"/> instance which stores data at the given
    /// <paramref name="basePath"/>.
    /// </summary>
    /// <param name="basePath">
    /// The path to the directory where all data should be stored.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="basePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="basePath"/> is not a valid path.
    /// </exception>
    public DiskStorageProvider(string basePath)
    {
        _ = basePath ?? throw new ArgumentNullException(nameof(basePath));

        if (!PathUtils.TryGetFullPath(basePath, out var fullBasePath))
        {
            throw new ArgumentException($"{nameof(basePath)} is not a valid path.", nameof(basePath));
        }

        BasePath = fullBasePath;
    }

    /// <inheritdoc/>
    public override StorageContainer GetContainer(StorageContainerPath path) =>
        new DiskStorageContainer(this, path);
}
