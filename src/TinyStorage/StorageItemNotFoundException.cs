namespace TinyStorage;

using System;

/// <summary>
/// An exception thrown when a required storage item does not exist.
/// </summary>
[Serializable]
public class StorageItemNotFoundException : StorageException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageItemNotFoundException"/> class with a default error message.
    /// </summary>
    public StorageItemNotFoundException()
        : this(message: null, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageItemNotFoundException"/> class with the specified error message.
    /// </summary>
    /// <inheritdoc/>
    public StorageItemNotFoundException(string? message)
        : this(message, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageItemNotFoundException"/>
    /// class with the specified error message and and another exception which was the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public StorageItemNotFoundException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(innerException is not null), innerException) { }

    private static string GetDefaultMessage(bool hasInner) =>
        hasInner
            ? "The storage item or one of its parent containers does not exist. See the inner exception for details."
            : "The storage item or one of its parent containers does not exist.";
}
