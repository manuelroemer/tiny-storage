namespace TinyStorage;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Represents a path that identifies a given <see cref="StorageContainer"/>.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public readonly struct StorageContainerPath : IEquatable<StorageContainerPath?>
{
    /// <summary>
    /// A <see cref="StorageContainerPath"/> pointing to the root container.
    /// </summary>
    public static readonly StorageContainerPath Root = new();

    /// <summary>
    /// Gets whether this path identifies the root container.
    /// </summary>
    public bool IsRoot => Segments.Count == 0;

    /// <summary>
    /// Gets the individual segments that, together, make up the entire
    /// path identifying a <see cref="StorageContainerPath"/>.
    /// </summary>
    public IReadOnlyList<string> Segments { get; }

    /// <summary>
    /// Initializes a new <see cref="StorageContainerPath"/> pointing to the root container.
    /// </summary>
    /// <remarks>
    /// It is recommended to use the <see cref="Root"/> instead of this constructor.
    /// </remarks>
    public StorageContainerPath()
        : this(Array.Empty<string>()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageContainerPath"/> class from
    /// the specified <paramref name="segment"/>.
    /// </summary>
    /// <param name="segment">The segment that makes up the entire path.</param>
    public StorageContainerPath(string segment)
        : this([segment ?? throw new ArgumentNullException(nameof(segment))]) { }

    /// <inheritdoc cref="StorageContainerPath(IEnumerable{string})"/>
    public StorageContainerPath(params string[] segments)
        : this((IEnumerable<string>)segments ?? throw new ArgumentNullException(nameof(segments))) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageContainerPath"/> class from
    /// the specified <paramref name="pathSegments"/>.
    /// </summary>
    /// <param name="pathSegments">
    /// The individual segments that, together, make up the entire path identifying a <see cref="StorageContainerPath"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="pathSegments"/> contains invalid segment strings.
    /// </exception>
    public StorageContainerPath(IEnumerable<string> pathSegments)
    {
        _ = pathSegments ?? throw new ArgumentNullException(nameof(pathSegments));

        var segmentsList = new ReadOnlyCollection<string>(pathSegments.ToList());

        if (segmentsList.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                $"A {nameof(StorageContainerPath)} cannot be initialized with a segment that is null, empty or whitespace.",
                nameof(pathSegments));
        }

        Segments = segmentsList;
    }

    /// <summary>
    /// Returns the parent path of this path.
    /// </summary>
    /// <returns>
    /// A <see cref="StorageContainerPath"/> which represents the parent of this path or
    /// <see langword="null"/> if this path is the root path.
    /// </returns>
    public StorageContainerPath? GetParent()
    {
        if (IsRoot)
        {
            return null;
        }

        if (Segments.Count == 1)
        {
            return Root;
        }

        var parentSegments = Segments.Take(Segments.Count - 1);
        return new StorageContainerPath(parentSegments);
    }

    /// <summary>
    /// Appends a new segment to this path.
    /// </summary>
    /// <param name="segment">The segment to be appended to this path.</param>
    /// <returns>
    /// A new <see cref="StorageContainerPath"/> instance that is a combination of this path
    /// and the specified <paramref name="segment"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="segment"/> is an invalid segment string.
    /// </exception>
    public StorageContainerPath Append(string segment)
    {
        _ = segment ?? throw new ArgumentNullException(nameof(segment));
        return Append((IEnumerable<string>)[segment]);
    }

    /// <inheritdoc cref="Append(IEnumerable{string})"/>
    public StorageContainerPath Append(params string[] segments)
    {
        _ = segments ?? throw new ArgumentNullException(nameof(segments));
        return Append((IEnumerable<string>)segments);
    }

    /// <summary>
    /// Returns a new path that is a concatenation of this path and the specified <paramref name="segments"/>.
    /// </summary>
    /// <param name="segments">The segments to be appended to this path.</param>
    /// <returns>
    /// A new <see cref="StorageContainerPath"/> instance that is a combination of this path
    /// and the specified <paramref name="segments"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="segments"/> contains invalid segment strings.
    /// </exception>
    public StorageContainerPath Append(IEnumerable<string> segments)
    {
        _ = segments ?? throw new ArgumentNullException(nameof(segments));
        var finalSegments = Segments.Concat(segments);
        return new StorageContainerPath(finalSegments);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is StorageContainerPath other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(StorageContainerPath? other) =>
        Equals(other, StringComparer.Ordinal);

    /// <summary>
    /// Determines whether this path is equal to the specified <paramref name="other"/> path
    /// by comparing both path segments using the specified <paramref name="stringComparer"/>.
    /// </summary>
    /// <param name="other">The other path.</param>
    /// <param name="stringComparer">
    /// The string comparer to be used for comparing the individual path segments.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if this path is equal to the specified <paramref name="other"/> path;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public bool Equals(StorageContainerPath? other, StringComparer stringComparer) =>
        other is not null && Segments.SequenceEqual(other.Value.Segments, stringComparer);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        GetHashCode(StringComparer.Ordinal);

    /// <summary>
    /// Returns the path's hash code, based on the individual segments and using the specified
    /// <paramref name="stringComparer"/>.
    /// </summary>
    /// <param name="stringComparer">
    /// The string comparer to be used for calculating the hash code.
    /// </param>
    /// <returns>
    /// The hash code of this path.
    /// </returns>
    public int GetHashCode(StringComparer stringComparer)
    {
        var hash = new HashCode();

        foreach (var segment in Segments)
        {
            hash.Add(segment, stringComparer);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Returns a string representation of this path.
    /// Joins all <see cref="Segments"/> with a slash <c>'/'</c> as separator.
    /// </summary>
    /// <returns>The path's segments joined with a slash <c>'/'</c> as separator.</returns>
    public override string ToString() =>
        ToString('/');

    /// <summary>
    /// Returns a string representation of this path.
    /// Joins the <see cref="Segments"/> with the specified <paramref name="separator"/>.
    /// </summary>
    /// <param name="separator">The separator to be used for joining the <see cref="Segments"/>.</param>
    /// <returns>The path's segments joined with the specified <paramref name="separator"/>.</returns>
    public string ToString(char separator) =>
        string.Join(separator.ToString(), Segments);

    /// <see cref="Append(string)"/>
    public static StorageContainerPath operator /(StorageContainerPath path, string segment) =>
        path.Append(segment);

    public static bool operator ==(StorageContainerPath? left, StorageContainerPath? right) =>
        left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(StorageContainerPath? left, StorageContainerPath? right) =>
        !(left == right);
}
