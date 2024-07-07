namespace Il.ClassViewRendering.Attributes;

/// <summary>
/// Registers view virtually following specified path.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class VirtualViewPathAttribute : Attribute
{
    public string Path { get; }

    public VirtualViewPathAttribute(string fullPath)
    {
        Path = fullPath;
    }

    public VirtualViewPathAttribute(string viewPath, string viewName)
    {
        Path = viewPath + viewName;
    }
}