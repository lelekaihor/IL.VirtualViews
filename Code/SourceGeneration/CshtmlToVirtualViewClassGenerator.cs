using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace IL.VirtualViews.SourceGeneration;

[Generator]
public sealed class CshtmlToVirtualViewClassGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var cshtmlFiles = context
            .AdditionalTextsProvider
            .Where(at => at.Path.EndsWith(".virtual.cshtml"))
            .Select((at, _) => at);

        context.RegisterSourceOutput(cshtmlFiles, (spc, cshtmlFile) =>
        {
            var className = Path.GetFileNameWithoutExtension(cshtmlFile.Path);
            className = className.Replace(".virtual", string.Empty);
            var content = cshtmlFile.GetText(spc.CancellationToken)?.ToString() ?? string.Empty;
            var pathSplit = Path
                .GetRelativePath("./", cshtmlFile.Path)
                .Split('\\');
            var namespaceToUse = string.Join('.', pathSplit[1..^1]);
            var source = GenerateClassSource(className, content, namespaceToUse);
            spc.AddSource($"{className}", SourceText.From(source, Encoding.UTF8));
        });
    }

    private static string GenerateClassSource(string className, string content, string namespaceToUse)
    {
        var escapedContent = content.Replace("\"", "\"\"");
        return
$@"using IL.VirtualViews.Interfaces;

namespace {namespaceToUse};

public partial class {className} : IVirtualView
{{
    public string ViewContent => @""
{escapedContent}
"";
}}";
    }
}