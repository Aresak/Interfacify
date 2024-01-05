using Aresak.Interfacify.Data;
using Aresak.Interfacify.Templates;
using Aresak.Interfacify.Templates.Observable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aresak.Interfacify;

[Generator]
public class InterfacifyGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_PATH = "Aresak.Interfacify.InterfacifyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = CreateProvider(context);
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = CreateCompilation(context, provider);

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Miscellaneous Design", "AV1210:Catch a specific exception instead of Exception, SystemException or ApplicationException", Justification = "<Pending>")]
    static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> nodes)
    {
        foreach (ClassDeclarationSyntax node in nodes)
        {
            try
            {
                ProcessNode(context, compilation, node);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    Debugger.Launch();
                }
            }
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

        FileTemplate template = GetTemplate(symbol);
        string source = template.GenerateFile();

        SaveSource(context, symbol, source);
    }

    static FileTemplate GetTemplate(INamedTypeSymbol symbol)
    {
        Template templateChoice = Template.Basic;

        AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass?.Name == nameof(InterfacifyAttribute));

        if (attribute is not null)
        {
            templateChoice = GetTemplateFromAttribute(attribute);
        }

        ClassMetadata metadata = new(symbol);
        FileTemplate template = GetTemplateFromChoice(templateChoice, metadata);

        return template;
    }

    static void SaveSource(SourceProductionContext context, INamedTypeSymbol symbol, string sourceText)
    {
        string originalName = symbol.ToDisplayString();
        SourceText source = SourceText.From(sourceText, Encoding.UTF8);
        context.AddSource($"{originalName}.Interfacify.g.cs", source);
    }

    static AttributeData GetInterfacifyAttribute(INamedTypeSymbol symbol)
    {
        AttributeData attribute = symbol.GetAttributes().First(attribute => attribute.AttributeClass?.Name == nameof(InterfacifyAttribute));
        return attribute;
    }

    static Template GetTemplateFromAttribute(AttributeData attribute)
    {
        Template template = Template.Basic;

        TypedConstant? templateArgument = attribute.ConstructorArguments.FirstOrDefault();

        if (templateArgument is not null)
        {
            string? value = templateArgument.Value.Value?.ToString();

            if (value is not null)
            {
                template = (Template)Enum.Parse(typeof(Template), value);
            }
        }

        return template;
    }

    static FileTemplate GetTemplateFromChoice(Template templateChoice, ClassMetadata metadata)
    {
        FileTemplate template = templateChoice switch
        {
            Template.Basic => new FileTemplate(metadata),
            Template.NotifyPropertyChanged => new ObservableFileTemplate(metadata),
            _ => throw new NotImplementedException($"Template choice '{templateChoice}' is not available"),
        };

        return template;
    }
}
