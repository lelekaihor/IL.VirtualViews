using System.Collections.Frozen;
using System.Reflection;
using Il.ClassViewRendering.Attributes;
using Il.ClassViewRendering.Helpers;
using Il.ClassViewRendering.Interfaces;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Il.ClassViewRendering.ContentProvider;

public sealed class VirtualViewsProvider : IFileProvider
{
    private FrozenDictionary<string, string> SupportedTypes { get; }

    public VirtualViewsProvider()
    {
        SupportedTypes = TypesAndAssembliesHelper
            .GetAssemblies("*")
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(TypesAndAssembliesHelper.GetExportedTypes)
            .Where(type => type is { IsAbstract: false, IsGenericTypeDefinition: false } && type.GetCustomAttribute<VirtualViewPathAttribute>() != default)
            .ToFrozenDictionary(
                keySelector => keySelector.GetCustomAttribute<VirtualViewPathAttribute>()!.Path,
                valueSelector =>
                {
                    var instance = Activator.CreateInstance(valueSelector) as IVirtualView;
                    return instance!.ViewContent;
                }
            );
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        // Not needed
        return new NotFoundDirectoryContents();
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (SupportedTypes.TryGetValue(subpath, out var content))
        {
            return new InMemoryFileInfo(subpath, content);
        }

        return new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        // Not needed
        return NullChangeToken.Singleton;
    }
}

public sealed class InMemoryFileInfo(string path, string content) : IFileInfo
{
    public string Name { get; } = Path.GetFileName(path);
    public bool Exists => true;
    public long Length => content.Length;
    public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
    public string PhysicalPath => null!;
    public bool IsDirectory => false;

    public Stream CreateReadStream()
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
    }
}