namespace IL.VirtualViews.Generation.Models;

public record GenerationClass(string Name, string CshtmlContent, string NamespaceToUse)
{
    public string Name { get; } = Name;
    public string CshtmlContent { get; } = CshtmlContent;

    public string NamespaceToUse { get; } = NamespaceToUse;
}