namespace TinyStorage.Tests;

using System.Collections.Generic;
using Xunit;

public abstract class StorageProviderImplTestsBase
{
    protected abstract StorageProvider Provider { get; }

    protected abstract IEnumerable<StorageContainerPath> InvalidContainerPaths { get; }

    [Fact]
    public void GetContainer_ThrowsInvalidStorageContainerPathExceptionForInvalidPaths()
    {
        foreach (var path in InvalidContainerPaths)
        {
            Assert.Throws<InvalidStorageContainerPathException>(() => Provider.GetContainer(path));
        }
    }
}
