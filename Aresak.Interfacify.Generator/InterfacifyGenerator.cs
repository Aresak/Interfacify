using Aresak.Interfacify.Generator.Attributes;
using Aresak.Interfacify.Generator.Extensions;
using Aresak.Interfacify.Generator.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aresak.Interfacify.Generator;

[Generator]
public class InterfacifyGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_PATH = "Aresak.Interfacify.Generator.Attributes.InterfacifyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = CreateProvider(context);
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = CreateCompilation(context, provider);

        Debugger.Launch();

        context.RegisterSourceOutput(compilation, (sourceContext, source) => Execute(sourceContext, source.Left, source.Right));
    }

    static IncrementalValuesProvider<ClassDeclarationSyntax> CreateProvider(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(ATTRIBUTE_PATH, ProviderPredicate, ProviderTransform)
            .Where(node => node is not null);

        return provider;
    }

    static ClassDeclarationSyntax ProviderTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        return (ClassDeclarationSyntax)context.TargetNode;
    }

    static bool ProviderPredicate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax;
    }

    static IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)>
        CreateCompilation(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ClassDeclarationSyntax> provider)
    {
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> nodes = provider.Collect();
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = context.CompilationProvider.Combine(nodes);

        return compilation;
    }

    static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> nodes)
    {
        foreach (ClassDeclarationSyntax node in nodes)
        {
            ProcessNode(context, compilation, node);
        }
    }

    static INamedTypeSymbol? GetSymbol(Compilation compilation, ClassDeclarationSyntax node)
    {
        INamedTypeSymbol? symbol = compilation
            .GetSemanticModel(node.SyntaxTree)
            .GetDeclaredSymbol(node) as INamedTypeSymbol;

        return symbol;
    }

    static void ProcessNode(SourceProductionContext context, Compilation compilation, ClassDeclarationSyntax node)
    {
        INamedTypeSymbol? symbol = GetSymbol(compilation, node);

        if (symbol is null)
        {
            return;
        }

        PropertyTemplate template = GetTemplate(symbol);
        GeneratedFileBuilder fileBuilder = new(symbol, template);

        string source = fileBuilder.Generate();

        SaveSource(context, symbol, source);
    }

    static PropertyTemplate GetTemplate(INamedTypeSymbol symbol)
    {
        AttributeData attribute = symbol.GetAttributes().First(attribute => attribute.AttributeClass?.Name == nameof(InterfacifyAttribute));
        // TODO: Use the reflection to get an actual template from the attribute
        return new();
    }

    static void SaveSource(SourceProductionContext context, INamedTypeSymbol symbol, string sourceText)
    {
        string originalName = symbol.ToDisplayString();
        SourceText source = SourceText.From(sourceText, Encoding.UTF8);
        context.AddSource($"{originalName}.g.cs", source);
    }
}
