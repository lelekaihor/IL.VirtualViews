namespace IL.VirtualViews.Attributes;

/// <summary>
/// Registers view virtually following specified path.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class VirtualViewPathAttribute : Attribute
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