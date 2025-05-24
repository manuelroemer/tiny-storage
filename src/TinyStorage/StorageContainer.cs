namespace TinyStorage;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a directory-like container in which both nested containers and files can be stored.
/// </summary>
[DebuggerDisplay("{Path}")]
public abstract class StorageContainer
{
    /// <summary>
    /// Gets the <see cref="StorageProvider"/> that this container is associated with.
    /// </summary>
    public StorageProvider Provider { get; }

    /// <summary>
    /// Gets the <see cref="StorageContainerPath"/> that identifies this container.
    /// </summary>
    public StorageContainerPath Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageContainer"/> class.
    /// </summary>
    /// <param name="provider">
    /// The <see cref="StorageProvider"/> that this container is associated with.
    /// </param>
    /// <param name="path">
    /// The <see cref="StorageContainerPath"/> that identifies this container.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    protected StorageContainer(StorageProvider provider, StorageContainerPath path)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Path = path;
    }

    /// <summary>
    /// Returns whether the container exists.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the container exists; <see langword="false"/> if not.
    /// </returns>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    public abstract Task<bool> ExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns whether the container contains a file with the given name.
    /// </summary>
    /// <param name="fileName">
    /// The name of the file.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the container contains a file with the given name; <see langword="false"/> if not.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fileName"/> is invalid.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    public abstract Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates the container and all of its parents if they do not exist yet.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    public abstract Task CreateIfNotExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a set of the file names of all files contained in this container.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A set of all file names contained in this container.
    /// </returns>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    /// <exception cref="StorageItemNotFoundException">
    /// The container does not exist.
    /// </exception>
    public abstract Task<IEnumerable<string>> ListFilesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a set of the container names of all containers contained in this container.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A set of all container names contained in this container.
    /// </returns>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    /// <exception cref="StorageItemNotFoundException">
    /// The container does not exist.
    /// </exception>
    public abstract Task<IEnumerable<string>> ListContainersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a file contained in this container with read access.
    /// </summary>
    /// <param name="fileName">
    /// The name of the file.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="Stream"/> that provides read access to the file.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fileName"/> is invalid.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    /// <exception cref="StorageItemNotFoundException">
    /// The file or the container does not exist.
    /// </exception>
    public abstract Task<Stream> OpenReadAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a file contained in this container with write access.
    /// </summary>
    /// <param name="fileName">
    /// The name of the file.
    /// </param>
    /// <param name="overwrite">
    /// <see langword="true"/> to overwrite the file if it already exists;
    /// <see langword="false"/> to throw an exception if it already exists.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="Stream"/> that provides write access to the file.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fileName"/> is invalid.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    /// <exception cref="StorageItemNotFoundException">
    /// The file or the container does not exist.
    /// </exception>
    public abstract Task<Stream> OpenWriteAsync(
        string fileName,
        bool overwrite,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the container and all contained items.
    ///
    /// This method does not throw if the container does not exist.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    public abstract Task DeleteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contained file.
    ///
    /// This method does not throw if the file or container does not exist.
    /// </summary>
    /// <param name="fileName">
    /// The name of the file.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fileName"/> is invalid.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error caused by the underlying storage medium prevented the method from completing.
    /// </exception>
    public abstract Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public sealed override string ToString() =>
        Path.ToString();

    /// <summary>
    /// Appends a new <paramref name="segment"/> to the container's current <see cref="Path"/> and
    /// returns the resulting <see cref="StorageContainer"/>.
    ///
    /// See <see cref="StorageContainerPath.Append(string)"/> for details.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    /// <param name="segment">The segment to be appended to this container's path.</param>
    /// <returns>
    /// A new <see cref="StorageContainer"/> with this container's <see cref="Path"/> plus
    /// the appended <paramref name="segment"/>.
    /// </returns>
    public static StorageContainer operator /(StorageContainer container, string segment)
    {
        var newPath = container.Path.Append(segment);
        return container.Provider.GetContainer(newPath);
    }
}
