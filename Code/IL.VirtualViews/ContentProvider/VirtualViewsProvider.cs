#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Immutable;
using System.Reflection;
using IL.Misc.Helpers;
using IL.VirtualViews.Attributes;
using IL.VirtualViews.Interfaces;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace IL.VirtualViews.ContentProvider;

public sealed class VirtualViewsProvider : IFileProvider
{
#if NET8_0_OR_GREATER
    private FrozenDictionary<string, string> SupportedTypes { get; }
#else
    private ImmutableDictionary<string, string> SupportedTypes { get; }
#endif

    public VirtualViewsProvider()
    {
        SupportedTypes = TypesAndAssembliesHelper
            .GetAssemblies("*")
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(TypesAndAssembliesHelper.GetExportedTypes)
            .Where(type => type is { IsAbstract: false, IsGenericTypeDefinition: false }
                           && type.GetCustomAttribute<VirtualViewPathAttribute>() != default
                           && HasIVirtualViewInterfaceMethod(type))

#if NET8_0_OR_GREATER
            .ToFrozenDictionary(
#else
            .ToImmutableDictionary(
#endif
                keySelector => keySelector.GetCustomAttribute<VirtualViewPathAttribute>()!.Path,
                valueSelector =>
                {
                    var instance = Activator.CreateInstance(valueSelector) as IVirtualView;
                    return instance!.ViewContent();
                }
            );
    }

    private static bool HasIVirtualViewInterfaceMethod(Type type)
    {
        return type.GetMethods().Any(x => x.Name == nameof(IVirtualView.ViewContent));
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

public sealed class InMemoryFileInfo : IFileInfo
{
    private readonly string _content;

    public InMemoryFileInfo(string path, string content)
    {
        _content = content;
        Name = Path.GetFileName(path);
    }

    public string Name { get; }

    public bool Exists => true;

    public long Length => _content.Length;

    public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

    public string PhysicalPath => null!;

    public bool IsDirectory => false;

    public Stream CreateReadStream()
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_content));
    }
}