namespace TinyStorage.Disk;

using System.Diagnostics.CodeAnalysis;
using System.IO;

internal static class PathUtils
{
    public static bool TryGetFullPath(string? path, [NotNullWhen(true)] out string? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            return false;
        }

        try
        {
            result = Path.GetFullPath(path);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}
