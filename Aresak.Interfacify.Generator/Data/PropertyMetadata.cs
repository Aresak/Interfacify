using Microsoft.CodeAnalysis;

namespace Aresak.Interfacify.Data;

/// <summary>
/// Metadata for a property.
/// </summary>
internal record PropertyMetadata : Metadata
{
    /// <summary>
    /// Type of the property.
    /// </summary>
    public ITypeSymbol Type;

    /// <summary>
    /// Whether the property has a getter.
    /// </summary>
    public bool HasGetter;

    /// <summary>
    /// Whether the property has a setter.
    /// </summary>
    public bool HasSetter;

    /// <summary>
    /// Creates a new instance of <see cref="PropertyMetadata"/>.
    /// </summary>
    /// <param name="property">Property to use</param>
    public PropertyMetadata(IPropertySymbol property)
    {
        Accessibility = property.DeclaredAccessibility;
        Name = property.Name;
        Type = property.Type;
        HasGetter = property.GetMethod != null;
        HasSetter = property.SetMethod != null;
    }
}