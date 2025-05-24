namespace TinyStorage.Tests;

using System;
using NSubstitute;
using Xunit;

public sealed class StorageContainerTests
{
    public StorageProvider Provider { get; } = Substitute.For<StorageProvider>();

    [Fact]
    public void Constructor_SetsProviderAndPath()
    {
        var path = new StorageContainerPath("1", "2", "3");
        var container = Substitute.ForPartsOf<StorageContainer>(Provider, path);
        Assert.Equal(Provider, container.Provider);
        Assert.Equal(path, container.Path);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException()
    {
        var ex = Record.Exception(() => Substitute.For<StorageContainer>(null, default));
        Assert.IsType<ArgumentNullException>(ex?.InnerException);
    }

    [Fact]
    public void ToString_ReturnsPathToString()
    {
        var path = new StorageContainerPath("1", "2", "3");
        var container = Substitute.ForPartsOf<StorageContainer>(Provider, path);
        Assert.Equal(path.ToString(), container.ToString());
    }

    [Fact]
    public void Append_ReturnsNewStorageContainer()
    {
        var path = new StorageContainerPath("1");
        var container = Substitute.For<StorageContainer>(Provider, path);
        var appended = container / "2";
        Provider.Received(1).GetContainer(new StorageContainerPath("1", "2"));
    }
}
