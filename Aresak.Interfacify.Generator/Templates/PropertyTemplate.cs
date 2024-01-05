using Aresak.Interfacify.Data;

namespace Aresak.Interfacify.Templates;

internal class PropertyTemplate(PropertyMetadata property)
{
    protected PropertyMetadata Property => property;

    public virtual string Generate()
    {
        if (Property.HasSetter && Property.HasGetter)
        {
            return GenerateWithGetterSetter();
        }
        else if (Property.HasGetter)
        {
            return GenerateWithGetterOnly();
        }
        else if (Property.HasSetter)
        {
            return GenerateWithSetterOnly();
        }
        else
        {
            return GenerateEmpty();
        }
    }

    protected virtual string GenerateWithGetterOnly()
    {
        string declaration = GeneratePropertyDeclaration();

        return $@"
            {declaration} {{
                get;
            }}
            ";
    }

    protected virtual string GenerateWithSetterOnly()
    {
        // The Only set will give CS8051 error. It needs to have get accessors, or set the value to something
        return GenerateWithGetterSetter();
    }

    protected virtual string GenerateWithGetterSetter()
    {
        string declaration = GeneratePropertyDeclaration();

        return $@"
            {declaration} {{
                get;
                set;
            }}
            ";
    }

    protected virtual string GenerateEmpty()
    {
        string declaration = GeneratePropertyDeclaration();

        return $"{declaration};";
    }

    protected virtual string GeneratePropertyDeclaration()
    {
        return $"{Property.AccessibilityToString()} {Property.Type.Name} {Property.Name}";
    }
}
