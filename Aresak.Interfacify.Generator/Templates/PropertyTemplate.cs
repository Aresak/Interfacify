using Aresak.Interfacify.Data;

namespace Aresak.Interfacify.Templates;

/// <summary>
/// Basic property template for the generated files.
/// </summary>
/// <param name="property"></param>
internal class PropertyTemplate(PropertyMetadata property)
{
    /// <summary>
    /// Metadata accessible also for other templates.
    /// </summary>
    protected PropertyMetadata Property => property;

    /// <summary>
    /// Generates the source code for the property.
    /// </summary>
    /// <returns>Full source code for the property</returns>
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

    /// <summary>
    /// Generate code for the property with only getter.
    /// </summary>
    /// <returns>Full source code for the property</returns>
    protected virtual string GenerateWithGetterOnly()
    {
        string declaration = GeneratePropertyDeclaration();

        return $@"
            {declaration} {{
                get;
            }}
            ";
    }

    /// <summary>
    /// Generate code for the property with only setter.
    /// </summary>
    /// <returns>Full source code for the property</returns>
    protected virtual string GenerateWithSetterOnly()
    {
        // The Only set will give CS8051 error. It needs to have get accessors, or set the value to something.
        return GenerateWithGetterSetter();
    }

    /// <summary>
    /// Generate code for the property with both getter and setter.
    /// </summary>
    /// <returns>Full source code for the property</returns>
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

    /// <summary>
    /// Generate code for property without specified get or set.
    /// </summary>
    /// <returns></returns>
    protected virtual string GenerateEmpty()
    {
        string declaration = GeneratePropertyDeclaration();

        return $"{declaration};";
    }

    /// <summary>
    /// Generates the property declaration.
    /// </summary>
    /// <returns>Property declaration ie. "private int Id"</returns>
    protected virtual string GeneratePropertyDeclaration()
    {
        string declaration = $"{Property.AccessibilityToString()} {Property.Type.ToDisplayString()} {Property.Name}";

        return declaration;
    }
}
