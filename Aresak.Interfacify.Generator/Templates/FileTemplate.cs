﻿using Aresak.Interfacify.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace Aresak.Interfacify.Templates;

/// <summary>
/// Basic file template for the generated files.
/// </summary>
/// <param name="metadata"></param>
internal class FileTemplate(ClassMetadata metadata)
{
    /// <summary>
    /// Metadata accessible also for other templates.
    /// </summary>
    protected ClassMetadata Metadata => metadata;

    /// <summary>
    /// Generates the source code for the file.
    /// </summary>
    /// <returns>Full source code for the file</returns>
    public virtual string GenerateFile()
    {
        string properties = GenerateProperties();
        string usings = AddUsingStatements();
        string attributes = AddClassAttributes();
        string additionalCode = AddAdditionalClassCode();

        // TODO: Specify the version in the [GeneratedCode] attribute
        // by Assembly version.

        string code = $@"
            //----------------------
            // <auto-generated>
            //     Interfacify
            // </auto-generated>
            //----------------------

            using System.CodeDom.Compiler;
            {usings}

            namespace {Metadata.Namespace}
            {{
                {attributes}
                [GeneratedCode(""Interfacify"", ""1.0.0"")]
                {Metadata.AccessibilityToString()} partial class {Metadata.Name}
                {{
                    {additionalCode}
                    {properties}
                }}
            }}
            ";

        code = ArrangeUsingRoslyn(code);

        return code;
    }

    /// <summary>
    /// Generates the source code for the property.
    /// </summary>
    /// <param name="property">Property to be generated</param>
    /// <returns>Full source code of the property</returns>
    protected virtual string GenerateProperty(PropertyMetadata property)
    {
        PropertyTemplate template = new(property);
        return template.Generate();
    }

    /// <summary>
    /// Generates the source code for the using statements.
    /// </summary>
    /// <returns>All usings that will be added to the full source code</returns>
    protected virtual string AddUsingStatements()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates the source code that will be placed in the class.
    /// </summary>
    /// <returns>Full source code that should be in the class</returns>
    protected virtual string AddAdditionalClassCode()
    {
        return string.Empty;
    }

    /// <summary>
    /// Genereates the source code for the class attributes.
    /// </summary>
    /// <returns>Full source code of attributes that will be prepended before the class definition</returns>
    protected virtual string AddClassAttributes()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates the source code for all properties.
    /// </summary>
    /// <returns>Full source code of all properties</returns>
    protected string GenerateProperties()
    {
        StringBuilder stringBuilder = new();

        foreach (PropertyMetadata property in Metadata.Properties)
        {
            string propertySource = GenerateProperty(property);
            stringBuilder.AppendLine(propertySource);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Arranges the source code using Roslyn.
    /// </summary>
    /// <param name="csCode">Full source code to be formatted</param>
    /// <returns>Formatted source code</returns>
    protected static string ArrangeUsingRoslyn(string csCode)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(csCode);
        SyntaxNode root = tree.GetRoot().NormalizeWhitespace();
        string ret = root.ToFullString();

        return ret;
    }
}
