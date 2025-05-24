namespace TinyStorage;

using System;

/// <summary>
/// An exception related to failed storage operations.
/// </summary>
[Serializable]
public class StorageException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/> class with a default error message.
    /// </summary>
    public StorageException()
        : this(message: null, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/> class with the specified error message.
    /// </summary>
    /// <inheritdoc/>
    public StorageException(string? message)
        : this(message, innerException: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/>
    /// class with the specified error message and and another exception which was the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public StorageException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(innerException is not null), innerException) { }

    private static string GetDefaultMessage(bool hasInner) =>
        hasInner
            ? "A storage error occurred. See the inner exception for details."
            : "A storage error occurred.";
}
