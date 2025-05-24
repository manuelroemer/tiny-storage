namespace TinyStorage.Tests.Disk;

using System;
using System.IO;
using TinyStorage.Disk;

public sealed class DiskStorageContainerTests : StorageContainerImplTestsBase, IDisposable
{
    protected override StorageProvider Provider { get; } = new DiskStorageProvider(
        Path.Join(Path.GetTempPath(), "TinyStorage", Guid.NewGuid().ToString()));

    public void Dispose()
    {
        var basePath = ((DiskStorageProvider)Provider).BasePath;
        if (Directory.Exists(basePath))
        {
            Directory.Delete(basePath, recursive: true);
        }
    }
}
