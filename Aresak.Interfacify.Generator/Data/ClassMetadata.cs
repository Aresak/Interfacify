using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Aresak.Interfacify.Data;

internal record ClassMetadata : Metadata
{
    public string Namespace = string.Empty;
    public List<PropertyMetadata> Properties = [];

    List<string> declaredMembers = [];

    public ClassMetadata(INamedTypeSymbol classSymbol)
    {
        Accessibility = classSymbol.DeclaredAccessibility;
        Name = classSymbol.Name;
        Namespace = classSymbol.ContainingNamespace.ToString();
        declaredMembers = classSymbol.MemberNames.ToList();

        ProcessInterfaceMembers(classSymbol);
    }

    void ProcessInterfaceMembers(INamedTypeSymbol classSymbol)
    {
        foreach (INamedTypeSymbol interfaceSymbol in classSymbol.AllInterfaces)
        {
            ProcessMembers(interfaceSymbol);
        }
    }

    void ProcessMembers(INamedTypeSymbol classSymbol)
    {
        ImmutableArray<ISymbol> members = classSymbol.GetMembers();

        foreach (ISymbol member in members)
        {
            if (member is not IPropertySymbol property || declaredMembers.Contains(property.Name))
            {
                continue;
            }

            AddProperty(property);
        }
    }

    void AddProperty(IPropertySymbol property)
    {
        PropertyMetadata propertyMetadata = new(property);
        Properties.Add(propertyMetadata);
        declaredMembers.Add(property.Name);
    }
}
