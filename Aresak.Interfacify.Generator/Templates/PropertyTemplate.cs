using Microsoft.CodeAnalysis;

namespace Aresak.Interfacify.Generator.Templates;

public class PropertyTemplate
{
    public string Generate(IPropertySymbol property)
    {
        if (property.SetMethod != null && property.GetMethod != null)
        {
            return GenerateWithGetterSetter(property);
        }
        else if (property.GetMethod != null)
        {
            return GenerateWithGetterOnly(property);
        }
        else if (property.SetMethod != null)
        {
            return GenerateWithSetterOnly(property);
        }
        else
        {
            return $"{property.DeclaredAccessibility.ToString().ToLowerInvariant()} {property.Type.Name} {property.Name};";
        }
    }

    protected string GenerateWithGetterOnly(IPropertySymbol property)
    {
        return $@"
            {property.DeclaredAccessibility.ToString().ToLowerInvariant()} {property.Type.Name} {property.Name} {{
                get;
            }}
            ";
    }

    protected string GenerateWithSetterOnly(IPropertySymbol property)
    {
        // The Only set will give CS8051 error. It needs to have get accessors, or set the value to something
        return $@"
            {property.DeclaredAccessibility.ToString().ToLowerInvariant()} {property.Type.Name} {property.Name} {{
                get;
                set;
            }}
            ";
    }

    protected string GenerateWithGetterSetter(IPropertySymbol property)
    {
        return $@"
            {property.DeclaredAccessibility.ToString().ToLowerInvariant()} {property.Type.Name} {property.Name} {{
                get;
                set;
            }}
            ";
    }
}
