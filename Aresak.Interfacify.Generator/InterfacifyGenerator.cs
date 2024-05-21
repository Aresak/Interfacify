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
        RegisterSourceOutput<ClassDeclarationSyntax>(context);
        RegisterSourceOutput<InterfaceDeclarationSyntax>(context);
    }

    static void RegisterSourceOutput<T>(IncrementalGeneratorInitializationContext context) where T : SyntaxNode
    {
        IncrementalValuesProvider<T> provider = CreateProvider<T>(context);
        IncrementalValueProvider<(Compilation Left, ImmutableArray<T> Right)> compilation = CreateCompilation(context, provider);

        context.RegisterSourceOutput(compilation, (sourceContext, source) => Execute(sourceContext, source.Left, source.Right));
    }

    /// <summary>
    /// Creates the provider for the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    static IncrementalValuesProvider<T> CreateProvider<T>(IncrementalGeneratorInitializationContext context) where T : SyntaxNode
    {
        IncrementalValuesProvider<T> provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(ATTRIBUTE_PATH, (node, _) => node is T, (context, _) => (T)context.TargetNode)
            .Where(node => node is not null);

        return provider;
    }

    /// <summary>
    /// Creates the compilation provider.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    static IncrementalValueProvider<(Compilation Left, ImmutableArray<T> Right)>
        CreateCompilation<T>(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<T> provider)
    {
        IncrementalValueProvider<ImmutableArray<T>> nodes = provider.Collect();
        IncrementalValueProvider<(Compilation Left, ImmutableArray<T> Right)> compilation = context.CompilationProvider.Combine(nodes);

        return compilation;
    }

    /// <summary>
    /// Processing of all classes implementing the <see cref="InterfacifyAttribute"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="nodes">A list of classes with the attribute</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Miscellaneous Design", "AV1210:Catch a specific exception instead of Exception, SystemException or ApplicationException", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1500:Member or local function contains too many statements", Justification = "<Pending>")]
    static void Execute<T>(SourceProductionContext context, Compilation compilation, ImmutableArray<T> nodes) where T : SyntaxNode
    {
        foreach (SyntaxNode node in nodes)
        {
            try
            {
                if (node is ClassDeclarationSyntax classNode)
                {
                    ProcessClassNode(context, compilation, classNode);
                }
                else if (node is InterfaceDeclarationSyntax interfaceNode)
                {
                    ProcessInterfaceNode(context, compilation, interfaceNode);
                }
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
    static INamedTypeSymbol? GetSymbol(Compilation compilation, SyntaxNode node)
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
    static void ProcessClassNode(SourceProductionContext context, Compilation compilation, ClassDeclarationSyntax node)
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
    /// Processes a single class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="node"></param>
    static void ProcessInterfaceNode(SourceProductionContext context, Compilation compilation, InterfaceDeclarationSyntax node)
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
