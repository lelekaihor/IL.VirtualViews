namespace IL.VirtualViews.Generation.Models;

public record GenerationClass(string Name, string CshtmlContent, string Path)
{
    public string Name { get; } = Name;
    public string CshtmlContent { get; } = CshtmlContent;

    public string Path { get; } = Path;
}