using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IL.VirtualViews.Generation.Models;
using Microsoft.CodeAnalysis;

namespace IL.VirtualViews.Generation.SourceGeneration;

[Generator]
public sealed class CshtmlToVirtualViewClassGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var cshtmlFiles = context
            .AdditionalTextsProvider
            .Where(at => at.Path.EndsWith(".virtual.cshtml"))
            .Select((cshtmlFile, cancellationToken) =>
            {
                var className = Path.GetFileNameWithoutExtension(cshtmlFile.Path);
                className = className.Replace(".virtual", string.Empty);
                var content = cshtmlFile.GetText(cancellationToken)?.ToString() ?? string.Empty;
                var pathSplit = GetRelativePath("./", cshtmlFile.Path)
                    .Split('\\')
                    .Select(PostProcessString)
                    .ToArray();
                var namespaceToUse = string.Join(".", pathSplit.Take(pathSplit.Length - 1));
                return new GenerationClass(PostProcessString(className), content, namespaceToUse);
            })
            .Collect();

        context.RegisterSourceOutput(cshtmlFiles, Generate);
    }
    
    private static string PostProcessString(string source)
    {
        // Trim leading and trailing whitespace
        var result = source.Trim();

        // Remove all whitespace characters (including spaces, tabs, newlines, etc.)
        result = Regex.Replace(result, @"\s+", string.Empty);

        // Optionally, remove or replace specific invisible characters
        // Here we replace zero-width space (U+200B), non-breaking space (U+00A0), etc.
        result = result.Replace("\u200B", string.Empty); // Zero-width space
        result = result.Replace("\u00A0", string.Empty); // Non-breaking space

        // Ensure proper encoding
        var bytes = Encoding.UTF8.GetBytes(result);
        result = Encoding.UTF8.GetString(bytes);

        return result;
    }

    private static void Generate(SourceProductionContext spc, ImmutableArray<GenerationClass> generationClasses)
    {
        foreach (var generationClass in generationClasses)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("using IL.VirtualViews.Interfaces;");
            sb.AppendLine();
            sb.AppendLine("namespace VirtualViews;"); 
            //sb.AppendLine($"namespace {generationClass.NamespaceToUse};");// TODO: investigate why actual namespace can't be used $"namespace {generationClass.NamespaceToUse};"
            sb.AppendLine();
            sb.AppendLine($"public partial class {generationClass.Name} : IVirtualView");
            sb.AppendLine($"{{");
            sb.AppendLine($"\tpublic string ViewContent()");
            sb.AppendLine($"\t{{");
            sb.AppendLine($"\t\treturn @\"{generationClass.CshtmlContent}\";");
            sb.AppendLine($"\t}}");
            sb.AppendLine($"}}");
            spc.AddSource($"{generationClass.Name}.g.cs", sb.ToString());
        }
    }
    private static string GetRelativePath(string relativeTo, string path)
    {
        relativeTo = Path.GetFullPath(relativeTo);
        var uriRelativeTo = new Uri(relativeTo);
        var uriPath = new Uri(path);

        return uriRelativeTo.MakeRelativeUri(uriPath).ToString().Replace('/', Path.DirectorySeparatorChar);
    }
}