using System;

namespace Aresak.Interfacify.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InterfacifyAttribute : Attribute
{
    public Template Template { get; }

    public InterfacifyAttribute(Template template)
    {
        Template = template;
    }

    public InterfacifyAttribute()
    {
        Template = Template.Basic;
    }
}
