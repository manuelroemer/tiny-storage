namespace TinyStorage;

using System;

/// <summary>
/// Provides access to <see cref="StorageContainer"/> instances which allow storing and retrieving
/// arbitrary, file system like data on an undefined storage medium.
/// </summary>
public abstract class StorageProvider
{
    /// <summary>
    /// Gets the provider's root <see cref="StorageContainer"/>.
    /// </summary>
    public StorageContainer RootContainer => GetContainer(StorageContainerPath.Root);

    /// <summary>
    /// <para>
    /// Returns a <see cref="StorageContainer"/> instance that represents the container at the specified path.
    /// The path is resolved via <paramref name="pathSelector"/>, starting with the <see cref="RootContainer"/>.
    /// </para>
    /// <para>
    /// This is a convenience overload which enables you to quickly resolve containers as follows:
    /// <c>GetContainer(root => root / "my" / "nested" / "path")</c>
    /// </para>
    /// </summary>
    /// <param name="pathSelector">
    /// A function receiving <see cref="StorageContainerPath.Root"/> as argument.
    /// The function must return the desired <see cref="StorageContainerPath"/> for the container to be returned.
    /// </param>
    /// <returns>
    /// A new <see cref="StorageContainer"/> instance identified by the path returned via <paramref name="pathSelector"/>.
    /// </returns>
    /// <exception cref="InvalidStorageContainerPathException">
    /// The resolved path is invalid.
    /// </exception>
    public StorageContainer GetContainer(Func<StorageContainerPath, StorageContainerPath> pathSelector)
    {
        _ = pathSelector ?? throw new ArgumentNullException(nameof(pathSelector));
        return GetContainer(pathSelector(StorageContainerPath.Root));
    }

    /// <summary>
    /// Returns a <see cref="StorageContainer"/> instance that represents the container at the specified path.
    /// </summary>
    /// <param name="path">A path that identifies a <see cref="StorageContainer"/>.</param>
    /// <returns>
    /// A new <see cref="StorageContainer"/> instance identified by the given <paramref name="path"/>.
    /// </returns>
    /// <exception cref="InvalidStorageContainerPathException">
    /// The given <paramref name="path"/> is invalid.
    /// </exception>
    public abstract StorageContainer GetContainer(StorageContainerPath path);
}
