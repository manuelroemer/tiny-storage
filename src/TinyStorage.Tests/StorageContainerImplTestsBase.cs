namespace TinyStorage.Tests;

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public abstract class StorageContainerImplTestsBase
{
    protected abstract StorageProvider Provider { get; }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseForNonExistentContainer()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        Assert.False(await container.ExistsAsync());
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueForExistingContainer()
    {
        var container = Provider.GetContainer(root => root / "existing");
        await container.CreateIfNotExistsAsync();
        Assert.True(await container.ExistsAsync());
    }

    [Fact]
    public async Task FileExistsAsync_ReturnsFalseForNonExistentFile()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        Assert.False(await container.FileExistsAsync("non-existent.txt"));
    }

    [Fact]
    public async Task FileExistsAsync_ReturnsTrueForExistingFile()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("existing.txt", overwrite: true)).Dispose();
        Assert.True(await container.FileExistsAsync("existing.txt"));
    }

    [Fact]
    public async Task FileExistsAsync_ThrowsArgumentNullException()
    {
        var container = Provider.RootContainer;
        await Assert.ThrowsAsync<ArgumentNullException>(() => container.FileExistsAsync(null!));
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_CreatesContainerIfNotExists()
    {
        var container = Provider.GetContainer(root => root / "new-container");
        Assert.False(await container.ExistsAsync());
        await container.CreateIfNotExistsAsync();
        Assert.True(await container.ExistsAsync());
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_DoesNotThrowIfContainerAlreadyExists()
    {
        var container = Provider.GetContainer(root => root / "existing-container");
        await container.CreateIfNotExistsAsync();
        await container.CreateIfNotExistsAsync();
        Assert.True(await container.ExistsAsync());
    }

    [Fact]
    public async Task ListFilesAsync_ReturnsFileNamesOfContainedFiles()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("file1.txt", overwrite: true)).Dispose();
        (await container.OpenWriteAsync("file2.txt", overwrite: true)).Dispose();

        var files = await container.ListFilesAsync();
        Assert.Equal(["file1.txt", "file2.txt"], files);
    }

    [Fact]
    public async Task ListFilesAsync_ReturnsEmptyEnumerableForEmptyContainer()
    {
        var container = Provider.GetContainer(root => root / "empty-container");
        await container.CreateIfNotExistsAsync();
        var files = await container.ListFilesAsync();
        Assert.Empty(files);
    }

    [Fact]
    public async Task ListFilesAsync_ThrowsStorageItemNotFoundExceptionForMissingContainer()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await Assert.ThrowsAsync<StorageItemNotFoundException>(() => container.ListFilesAsync());
    }

    [Fact]
    public async Task ListContainersAsync_ReturnsContainerNamesOfContainedContainers()
    {
        var container = Provider.RootContainer;
        var child1 = container / "child1";
        var child2 = container / "child2";
        await container.CreateIfNotExistsAsync();
        await child1.CreateIfNotExistsAsync();
        await child2.CreateIfNotExistsAsync();

        var containers = await container.ListContainersAsync();
        Assert.Equal(["child1", "child2"], containers);
    }

    [Fact]
    public async Task ListContainersAsync_ReturnsEmptyEnumerableForEmptyContainer()
    {
        var container = Provider.GetContainer(root => root / "empty-container");
        await container.CreateIfNotExistsAsync();
        var containers = await container.ListContainersAsync();
        Assert.Empty(containers);
    }

    [Fact]
    public async Task ListContainersAsync_ThrowsStorageItemNotFoundExceptionForMissingContainer()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await Assert.ThrowsAsync<StorageItemNotFoundException>(() => container.ListContainersAsync());
    }

    [Fact]
    public async Task OpenReadAsync_ReturnsStreamForReadingFile()
    {
        var container = Provider.RootContainer;
        var fileContent = "Hello world!";
        var fileName = "test.txt";
        await container.CreateIfNotExistsAsync();
        await using (var writeStream = await container.OpenWriteAsync(fileName, overwrite: true))
        {
            await using var writer = new StreamWriter(writeStream);
            await writer.WriteAsync(fileContent);
        }

        await using (var readStream = await container.OpenReadAsync(fileName))
        {
            using var reader = new StreamReader(readStream);
            var content = await reader.ReadToEndAsync();
            Assert.Equal(fileContent, content);
        }
    }

    [Fact]
    public async Task OpenReadAsync_ThrowsStorageItemNotFoundExceptionForMissingFile()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        await Assert.ThrowsAsync<StorageItemNotFoundException>(() => container.OpenReadAsync("non-existent.txt"));
    }

    [Fact]
    public async Task OpenReadAsync_ThrowsStorageItemNotFoundExceptionForMissingContainer()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await Assert.ThrowsAsync<StorageItemNotFoundException>(() => container.OpenReadAsync("file.txt"));
    }

    [Fact]
    public async Task OpenReadAsync_ThrowsArgumentNullException()
    {
        var container = Provider.RootContainer;
        await Assert.ThrowsAsync<ArgumentNullException>(() => container.OpenReadAsync(null!));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OpenWriteAsync_CreatesNewFile(bool overwrite)
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("file.txt", overwrite)).Dispose();
        Assert.True(await container.FileExistsAsync("file.txt"));
    }

    [Fact]
    public async Task OpenWriteAsync_TruncatesExistingFileForOverwriteTrue()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        using (var writeStream = await container.OpenWriteAsync("file.txt", overwrite: true))
        {
            await using var writer = new StreamWriter(writeStream);
            await writer.WriteAsync("Initial content");
        }

        using (var writeStream = await container.OpenWriteAsync("file.txt", overwrite: true))
        {
            await using var writer = new StreamWriter(writeStream);
            await writer.WriteAsync("New content");
        }

        using (var readStream = await container.OpenReadAsync("file.txt"))
        {
            using var reader = new StreamReader(readStream);
            var content = await reader.ReadToEndAsync();
            Assert.Equal("New content", content);
        }
    }

    [Fact]
    public async Task OpenWriteAsync_ThrowsStorageExceptionForExistingFileForOverwriteFalse()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("file.txt", overwrite: true)).Dispose();
        await Assert.ThrowsAsync<StorageException>(() => container.OpenWriteAsync("file.txt", overwrite: false));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OpenWriteAsync_ThrowsStorageItemNotFoundExceptionForMissingContainer(bool overwrite)
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await Assert.ThrowsAsync<StorageItemNotFoundException>(() => container.OpenWriteAsync("file.txt", overwrite));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OpenWriteAsync_ThrowsArgumentNullException(bool overwrite)
    {
        var container = Provider.RootContainer;
        await Assert.ThrowsAsync<ArgumentNullException>(() => container.OpenWriteAsync(null!, overwrite));
    }

    [Fact]
    public async Task DeleteAsync_DeletesContainerAndAllContent()
    {
        var container = Provider.GetContainer(root => root / "to-delete");
        var childContainer = container / "child";
        await container.CreateIfNotExistsAsync();
        await childContainer.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("file.txt", overwrite: true)).Dispose();
        (await childContainer.OpenWriteAsync("child-file.txt", overwrite: true)).Dispose();

        await container.DeleteAsync();

        Assert.False(await container.ExistsAsync());
        Assert.False(await childContainer.ExistsAsync());
        Assert.False(await container.FileExistsAsync("file.txt"));
        Assert.False(await childContainer.FileExistsAsync("child-file.txt"));
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrowIfContainerDoesNotExist()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await container.DeleteAsync();
        Assert.False(await container.ExistsAsync());
    }

    [Fact]
    public async Task DeleteFileAsync_DeletesFile()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        (await container.OpenWriteAsync("file.txt", overwrite: true)).Dispose();

        await container.DeleteFileAsync("file.txt");

        Assert.False(await container.FileExistsAsync("file.txt"));
    }

    [Fact]
    public async Task DeleteFileAsync_DoesNotThrowIfFileDoesNotExist()
    {
        var container = Provider.RootContainer;
        await container.CreateIfNotExistsAsync();
        await container.DeleteFileAsync("non-existent.txt");
        Assert.False(await container.FileExistsAsync("non-existent.txt"));
    }

    [Fact]
    public async Task DeleteFileAsync_DoesNotThrowIfContainerDoesNotExist()
    {
        var container = Provider.GetContainer(root => root / "non-existent");
        await container.DeleteFileAsync("file.txt");
        Assert.False(await container.ExistsAsync());
    }
}
