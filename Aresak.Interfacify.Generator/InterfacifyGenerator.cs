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

/// <summary>
/// Code generator for the <see cref="InterfacifyAttribute"/>.
/// </summary>
[Generator]
public class InterfacifyGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Full path for the InterfacifyAttribute.
    /// </summary>
    const string ATTRIBUTE_PATH = "Aresak.Interfacify.InterfacifyAttribute";

    /// <summary>
    /// Initializes the generator.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = CreateProvider(context);
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = CreateCompilation(context, provider);

        context.RegisterSourceOutput(compilation, (sourceContext, source) => Execute(sourceContext, source.Left, source.Right));
    }

    /// <summary>
    /// Creates the provider for the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    static IncrementalValuesProvider<ClassDeclarationSyntax> CreateProvider(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(ATTRIBUTE_PATH, ProviderPredicate, ProviderTransform)
            .Where(node => node is not null);

        return provider;
    }

    /// <summary>
    /// Casts ContextNode to ClassDeclarationSyntax.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    static ClassDeclarationSyntax ProviderTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        return (ClassDeclarationSyntax)context.TargetNode;
    }

    /// <summary>
    /// Predicate for the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    static bool ProviderPredicate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax;
    }

    /// <summary>
    /// Creates the compilation provider.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    static IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)>
        CreateCompilation(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ClassDeclarationSyntax> provider)
    {
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> nodes = provider.Collect();
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = context.CompilationProvider.Combine(nodes);

        return compilation;
    }

    /// <summary>
    /// Processing of all classes implementing the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="nodes">A list of classes with the attribute</param>
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
                // TODO: There might be a better way to handle this,
                // but this is my first generator, feel free to improve it.
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

    /// <summary>
    /// Gets the symbol for the class.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    static INamedTypeSymbol? GetSymbol(Compilation compilation, ClassDeclarationSyntax node)
    {
        INamedTypeSymbol? symbol = compilation
            .GetSemanticModel(node.SyntaxTree)
            .GetDeclaredSymbol(node) as INamedTypeSymbol;

        return symbol;
    }

    /// <summary>
    /// Processes a single class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="node"></param>
    static void ProcessNode(SourceProductionContext context, Compilation compilation, ClassDeclarationSyntax node)
    {
        INamedTypeSymbol? symbol = GetSymbol(compilation, node);

        if (symbol is null)
        {
            return;
        }

        // Select the correct template.
        FileTemplate template = GetTemplate(symbol);

        // Generate the source.
        string source = template.GenerateFile();

        SaveSource(context, symbol, source);
    }

    /// <summary>
    /// Gets the template to generate from the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    static FileTemplate GetTemplate(INamedTypeSymbol symbol)
    {
        AttributeData attribute = symbol.GetAttributes()
            .First(attribute => attribute.AttributeClass?.Name == nameof(InterfacifyAttribute));

        Template templateChoice = GetTemplateFromAttribute(attribute);

        ClassMetadata metadata = new(symbol);
        FileTemplate template = GetTemplateFromChoice(templateChoice, metadata);

        return template;
    }

    /// <summary>
    /// Saves the generated source.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="sourceText">The C# Source code to save</param>
    static void SaveSource(SourceProductionContext context, INamedTypeSymbol symbol, string sourceText)
    {
        string originalName = symbol.ToDisplayString();
        SourceText source = SourceText.From(sourceText, Encoding.UTF8);
        context.AddSource($"{originalName}.Interfacify.g.cs", source);
    }

    /// <summary>
    /// Get the template from the attribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    static Template GetTemplateFromAttribute(AttributeData attribute)
    {
        // Default to Basic.
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

    /// <summary>
    /// Instantiates the template from the choice.
    /// </summary>
    /// <param name="templateChoice">Specified template</param>
    /// <param name="metadata">Metadata of the class for the template</param>
    /// <returns>Template implementation</returns>
    /// <exception cref="NotImplementedException">Thrown if the Template isn't switched here</exception>
    static FileTemplate GetTemplateFromChoice(Template templateChoice, ClassMetadata metadata)
    {
        // Idea: Maybe use the reflection to instaniate the template?

        FileTemplate template = templateChoice switch
        {
            Template.Basic => new FileTemplate(metadata),
            Template.NotifyPropertyChanged => new ObservableFileTemplate(metadata),
            _ => throw new NotImplementedException($"Template choice '{templateChoice}' is not available"),
        };

        return template;
    }
}
