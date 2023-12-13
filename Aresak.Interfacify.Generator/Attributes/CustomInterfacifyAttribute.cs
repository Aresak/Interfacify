using Aresak.Interfacify.Generator.Templates;
using System;

namespace Aresak.Interfacify.Generator.Attributes;

public class CustomInterfacifyAttribute<TTemplate> : InterfacifyAttribute
{
    public CustomInterfacifyAttribute() : base(typeof(TTemplate))
    {

    }
}

[AttributeUsage(AttributeTargets.Class)]
public class InterfacifyAttribute : Attribute
{
    public Type Template { get; }

    public InterfacifyAttribute(Type template)
    {
        Template = template;
    }

    public InterfacifyAttribute()
    {
        Template = typeof(PropertyTemplate);
    }
}
