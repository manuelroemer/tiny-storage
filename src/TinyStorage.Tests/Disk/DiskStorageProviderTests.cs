namespace TinyStorage.Tests.Disk;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyStorage.Disk;

public sealed class DiskStorageProviderTests : StorageProviderImplTestsBase, IDisposable
{
    protected override StorageProvider Provider { get; } = new DiskStorageProvider(
        Path.Join(Path.GetTempPath(), "TinyStorage", Guid.NewGuid().ToString()));

    protected override IEnumerable<StorageContainerPath> InvalidContainerPaths =>
        new char[]
            {

                Path.PathSeparator,
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
                Path.VolumeSeparatorChar,
            }
            .Select(c => new StorageContainerPath($"{c}"));


    public void Dispose()
    {
        var basePath = ((DiskStorageProvider)Provider).BasePath;
        if (Directory.Exists(basePath))
        {
            Directory.Delete(basePath, recursive: true);
        }
    }
}
