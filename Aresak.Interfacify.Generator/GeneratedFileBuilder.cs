using Aresak.Interfacify.Generator.Templates;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Aresak.Interfacify.Generator;

internal class GeneratedFileBuilder(INamedTypeSymbol symbol, PropertyTemplate template)
{
    List<string> declaredMembers = symbol.MemberNames.ToList();
    List<IPropertySymbol> membersToGenerate = [];

    public string Generate()
    {
        foreach (INamedTypeSymbol interfaceType in symbol.AllInterfaces)
        {
            PrepareInterface(interfaceType);
        }

        string generatedSource = GenerateSource();

        return generatedSource;
    }

    string GenerateSource()
    {
        string propertiesSource = GeneratePropertiesSource();

        string namespaceSource = symbol.ContainingNamespace.ToDisplayString();
        return @$"""
            namespace {namespaceSource} {{

                {symbol.DeclaredAccessibility.ToString().ToLowerInvariant()} partial class {symbol.Name} {{
                    {propertiesSource}
                }}

            }}
            """;
    }

    private string GeneratePropertiesSource()
    {
        StringBuilder stringBuilder = new();

        foreach (IPropertySymbol property in membersToGenerate)
        {
            string propertySource = template.Generate(property);
            stringBuilder.AppendLine(propertySource);
        }

        return stringBuilder.ToString();
    }

    void PrepareInterface(INamedTypeSymbol interfaceType)
    {
        ImmutableArray<ISymbol> members = interfaceType.GetMembers();

        foreach (ISymbol member in members)
        {
            if (member is not IPropertySymbol propertySymbol)
            {
                continue;
            }

            PrepareMember(propertySymbol);
        }
    }

    void PrepareMember(IPropertySymbol member)
    {
        bool isMemberDeclared = IsMemberDeclared(member.Name);

        if (isMemberDeclared)
        {
            return;
        }

        DeclareMember(member);
    }

    bool IsMemberDeclared(string name)
    {
        return declaredMembers.Contains(name);
    }

    void DeclareMember(IPropertySymbol member)
    {
        membersToGenerate.Add(member);
        declaredMembers.Add(member.Name);

        //ImmutableArray<SymbolDisplayPart> memberInformation = member.ToDisplayParts();

        //string propertyName = memberInformation.First(part => part.Kind == SymbolDisplayPartKind.PropertyName).ToString();

        //GeneratedMember generatedMember = GetGeneratedMember(propertyName, member);
        //AssignKeywordToMember(generatedMember, memberInformation);
    }

    //GeneratedMember GetGeneratedMember(string name, Accessibility accessibility)
    //{
    //    GeneratedMember? member = membersToGenerate.FirstOrDefault(member => member.Name == name);

    //    if (member == null)
    //    {
    //        member = new(name, accessibility);
    //        membersToGenerate.Add(member);
    //        declaredMembers.Add(name);
    //    }

    //    return member;
    //}

    //void AssignKeywordToMember(GeneratedMember member, ImmutableArray<SymbolDisplayPart> memberInformation)
    //{
    //    string keyword = memberInformation.FirstOrDefault(part => part.Kind == SymbolDisplayPartKind.Keyword).ToString();

    //    if (keyword == "get")
    //    {
    //        member.Getter = string.Empty;
    //    }
    //    else if (keyword == "set")
    //    {
    //        member.Setter = string.Empty;
    //    }
    //    else
    //    {
    //        // Sad here
    //    }
    //}
}
