namespace TinyStorage;

using System;

/// <summary>
/// An exception thrown when a <see cref="StorageContainerPath"/> is invalid in a given context.
/// </summary>
public class InvalidStorageContainerPathException : StorageException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/> class with a default error message.
    /// </summary>
    public InvalidStorageContainerPathException()
        : this(message: null, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/> class with the specified error message.
    /// </summary>
    /// <inheritdoc/>
    public InvalidStorageContainerPathException(string? message)
        : this(message, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/>
    /// class with the specified error message and another exception which was the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public InvalidStorageContainerPathException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

    private static string GetDefaultMessage() =>
        "The specified storage path is invalid.";
}
