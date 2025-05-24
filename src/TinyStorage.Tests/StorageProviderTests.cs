namespace TinyStorage.Tests;

using NSubstitute;
using Xunit;

public sealed class StorageProviderTests
{
    [Fact]
    public void GetRootContainer_CallsGetContainerWithRootPath()
    {
        var provider = Substitute.For<StorageProvider>();
        var rootPath = StorageContainerPath.Root;
        _ = provider.RootContainer;
        provider.Received(1).GetContainer(rootPath);
    }

    [Fact]
    public void GetContainerWithPathSelector_CallsGetContainerWithResolvedPath()
    {
        var provider = Substitute.For<StorageProvider>();
        var path = new StorageContainerPath("my", "nested", "path");
        _ = provider.GetContainer(root => root / "my" / "nested" / "path");
        provider.Received(1).GetContainer(path);
    }
}
