namespace TinyStorage.Tests;

using System;
using System.Collections.Generic;
using Xunit;

public sealed class StorageContainerPathTests
{
    [Fact]
    public void Root_HasRootTraits()
    {
        var path = StorageContainerPath.Root;
        Assert.True(path.IsRoot);
        Assert.Empty(path.Segments);
        Assert.Null(path.GetParent());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false, "1")]
    [InlineData(false, "1", "2")]
    public void IsRoot_ReturnsTrueForPathWithoutSegments(bool isRoot, params string[] segments)
    {
        var path = new StorageContainerPath(segments);
        Assert.Equal(isRoot, path.IsRoot);
    }

    [Theory]
    [InlineData()]
    [InlineData("1")]
    [InlineData("1", "2")]
    public void Segments_ReturnsNewSegmentsCollectionWithSameContent(params string[] segments)
    {
        var path = new StorageContainerPath(segments);
        Assert.Equal(segments, path.Segments);
        Assert.NotSame(segments, path.Segments);
    }

    [Fact]
    public void ConstructorWithoutArgs_InitializesRootPath()
    {
        var path = new StorageContainerPath();
        Assert.True(path.IsRoot);
    }

    [Theory]
    [InlineData("1")]
    public void ConstructorWithString_InitializesWithSegment(string segment)
    {
        var path = new StorageContainerPath(segment);
        Assert.Equal([segment], path.Segments);
    }

    [Theory]
    [InlineData("1", "2")]
    public void ConstructorWithStringArray_InitializesWithSegments(params string[] segments)
    {
        var path = new StorageContainerPath(segments);
        Assert.Equal(segments, path.Segments);
    }

    [Theory]
    [InlineData("1", "2")]
    public void ConstructorWithEnumerable_InitializesWithSegments(params string[] segments)
    {
        var path = new StorageContainerPath((IEnumerable<string>)segments);
        Assert.Equal(segments, path.Segments);
    }

    [Fact]
    public void Constructors_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StorageContainerPath((string)null!));
        Assert.Throws<ArgumentNullException>(() => new StorageContainerPath((string[])null!));
        Assert.Throws<ArgumentNullException>(() => new StorageContainerPath((IEnumerable<string>)null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("valid", "")]
    public void Constructors_ThrowArgumentExceptionForAnyInvalidSegment(params string[] segments)
    {
        Assert.Throws<ArgumentException>(() => new StorageContainerPath(segments));
        Assert.Throws<ArgumentException>(() => new StorageContainerPath((IEnumerable<string>)segments));
    }

    [Fact]
    public void GetParent_ReturnsNullForRootPath()
    {
        var path = StorageContainerPath.Root;
        var parent = path.GetParent();
        Assert.Null(parent);
    }

    [Fact]
    public void GetParent_ReturnsRootPathForPathWithOneSegment()
    {
        var path = new StorageContainerPath("1");
        var parent = path.GetParent();
        Assert.Equal(StorageContainerPath.Root, parent);
    }

    [Fact]
    public void GetParent_ReturnsNewPathWithoutLastSegment()
    {
        var path = new StorageContainerPath("1", "2", "3");
        var parent = path.GetParent();
        Assert.Equal(["1", "2"], parent?.Segments);
    }

    [Fact]
    public void AppendString_ReturnsNewPathWithSegment()
    {
        var path = new StorageContainerPath("1");
        var newPath = path.Append("2");
        Assert.Equal(["1", "2"], newPath.Segments);
    }

    [Fact]
    public void AppendStringArray_ReturnsNewPathWithSegments()
    {
        var path = new StorageContainerPath("1");
        var newPath = path.Append(["2", "3"]);
        Assert.Equal(["1", "2", "3"], newPath.Segments);
    }

    [Fact]
    public void AppendStringEnumerable_ReturnsNewPathWithSegments()
    {
        var path = new StorageContainerPath("1");
        var newPath = path.Append((IEnumerable<string>)["2", "3"]);
        Assert.Equal(["1", "2", "3"], newPath.Segments);
    }

    [Fact]
    public void AppendOperator_ReturnsNewPathWithSegment()
    {
        var path = new StorageContainerPath("1");
        var newPath = path / "2";
        Assert.Equal(["1", "2"], newPath.Segments);
    }

    [Fact]
    public void AppendOverloads_ThrowArgumentNullException()
    {
        var path = new StorageContainerPath("1");
        Assert.Throws<ArgumentNullException>(() => path.Append((string)null!));
        Assert.Throws<ArgumentNullException>(() => path.Append((string[])null!));
        Assert.Throws<ArgumentNullException>(() => path.Append((IEnumerable<string>)null!));
        Assert.Throws<ArgumentNullException>(() => path / null!);
    }

    [Fact]
    public void EqualsObject_ReturnsEqualityValue()
    {
        var path = new StorageContainerPath("1");
        Assert.False(path.Equals(null));
        Assert.False(path.Equals("1"));
        Assert.False(path.Equals((object?)new StorageContainerPath("2")));
        Assert.True(path.Equals(path));
        Assert.True(path.Equals((object?)new StorageContainerPath("1")));
    }

    [Fact]
    public void EqualsStorageContainerPath_ReturnsEqualityValue()
    {
        var path = new StorageContainerPath("1");
        Assert.False(path.Equals(new StorageContainerPath("2")));
        Assert.False(path == new StorageContainerPath("2"));
        Assert.True(path.Equals(new StorageContainerPath("1")));
        Assert.True(path == new StorageContainerPath("1"));
    }

    [Fact]
    public void EqualsStorageContainerPath_ComparesSegmentsWithOrdinalStringComparison()
    {
        var path = new StorageContainerPath("abc");
        Assert.True(path.Equals(new StorageContainerPath("abc")));
        Assert.False(path.Equals(new StorageContainerPath("ABC")));
    }

    [Fact]
    public void EqualsStorageContainerPathWithComparer_ComparesSegmentsWithProvidedStringComparison()
    {
        var path = new StorageContainerPath("abc");
        Assert.True(path.Equals(new StorageContainerPath("ABC"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetHashCode_ReturnsHashCodeBasedOnSegmentsAndStringComparison()
    {
        var pathLower = new StorageContainerPath("abc");
        var pathUpper = new StorageContainerPath("ABC");
        Assert.Equal(pathLower.GetHashCode(), pathLower.GetHashCode());
        Assert.NotEqual(pathLower.GetHashCode(), pathUpper.GetHashCode());
        Assert.Equal(
            pathLower.GetHashCode(StringComparer.OrdinalIgnoreCase),
            pathUpper.GetHashCode(StringComparer.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1", "1")]
    [InlineData("1/2/3", "1", "2", "3")]
    public void ToString_ReturnsSegmentsSeparatedBySlash(string expected, params string[] segments)
    {
        var path = new StorageContainerPath(segments);
        Assert.Equal(expected, path.ToString());
    }
}
