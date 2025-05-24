# Tiny Storage

Tiny Storage is a minimal, DI-ready file/object storage abstraction that allows implementing a custom storage adapter within one hour.

## What and Why?

My personal use case for Tiny Storage is small, self-hosted web services that currently need to store data on the device's file system. As usual for .NET developers, I wanted an abstraction for "object storage" because those services might store data at another location like Azure Blob Storage or similar in the future (spoiler: that won't happen, but hey...).

Tiny Storage is a **minimal abstraction** for storing files at an arbitrary location.

## How Does It Work?

Tiny Storage's root interface is the `StorageProvider`. For example, to store files in a directory on your local file system, you can use the built-in `DiskStorageProvider`:

```csharp
using TinyStorage;
using TinyStorage.Disk;

StorageProvider storageProvider = new DiskStorageProvider("/tmp/my-storage");
```

Given a `StorageProvider`, you can access `StorageContainers` (directories/folders) via two functions:

```csharp
// RootContainer is the top-most container available.
StorageContainer rootContainer = storageProvider.RootContainer;
Console.WriteLine(rootContainer.Path); // <empty>

// Alternatively, get a specific container via a path:
StorageContainerPath path = new("some", "sub", "container");
StorageContainer container = storageProvider.GetContainer(path);
Console.WriteLine(container.Path); // some/sub/container
```

Both `StorageContainerPath` and the `StorageContainer` overload the `/` operator (C#) for convenient path building:

```csharp
StorageContainerPath myPath = StorageContainerPath.Root / "some" / "sub" / "container";
Console.WriteLine(myPath); // some/sub/container

StorageContainer container = storageProvider.RootContainer / "some" / "sub" / "container";
Console.WriteLine(container.Path); // some/sub/container

// Or alternatively, use GetContainer:
StorageContainer container = storageProvider.GetContainer(root => root / "some" / "sub" / "container");
Console.WriteLine(container.Path); // some/sub/container
```

Once you have a `StorageContainer` instance, you can use it to manipulate files:

```csharp
StorageContainer container = storageProvider.RootContainer;

await container.CreateIfNotExistsAsync();

bool containerExists = await container.ExistsAsync();
bool fileExists = await container.FileExistsAsync("file.txt");

IEnumerable<string> fileNames = await container.ListFilesAsync();
IEnumerable<string> subContainerNames = await container.ListContainersAsync();

using Stream readStream = await container.OpenReadAsync("r.txt");
using Stream writeStream = await container.OpenWriteAsync("w.txt", overwrite: true);

await container.DeleteFileAsync("file.txt");
await container.DeleteAsync();
```

## Dependency Injection

Tiny Storage plays well with dependency injection. Simply register your `StorageProvider` of choice in your DI container:

```csharp
// Using Microsoft.Extensions.DependencyInjection:
services.AddSingleton<StorageProvider>(new DiskStorageProvider("/tmp/my-storage"));
```

Then use the `StorageProvider` in your services:

```csharp
public class MyService
{
    private readonly StorageProvider _storageProvider;

    public MyService(StorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task CreateHelloWorldFile()
    {
        StorageContainer container = _storageProvider.RootContainer;
        await container.CreateIfNotExistsAsync();

        await using Stream writeStream = await container.OpenWriteAsync("hello.txt", overwrite: true);
        await using StreamWriter writer = new(writeStream);
        await writer.WriteLineAsync("Hello, World!");
    }
}
```

## Implement a Custom `StorageProvider`

To support a custom storage provider, you need to inherit from two classes:

1. `StorageContainer`: Implement file operations for your storage solution.
2. `StorageProvider`: Instantiate your custom `StorageContainer` instances.

For a reference implementation, check out the two disk storage implementations:
- [`DiskStorageContainer`](./src/TinyStorage/Disk/DiskStorageContainer.cs)
- [`DiskStorageProvider`](./src/TinyStorage/Disk/DiskStorageProvider.cs)

## Contributing

If you have any ideas, suggestions, fixes or improvements that you would like to see integrated into this project, feel free to open an issue or pull request!


## License

See the [LICENSE](./LICENSE) file for details.
