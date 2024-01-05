using Microsoft.CodeAnalysis;

namespace Aresak.Interfacify.Data;

internal record PropertyMetadata : Metadata
{
    public ITypeSymbol Type;
    public bool HasGetter;
    public bool HasSetter;

    public PropertyMetadata(IPropertySymbol property)
    {
        Accessibility = property.DeclaredAccessibility;
        Name = property.Name;
        Type = property.Type;
        HasGetter = property.GetMethod != null;
        HasSetter = property.SetMethod != null;
    }
}