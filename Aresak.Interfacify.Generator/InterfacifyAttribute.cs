using System;

namespace Aresak.Interfacify;

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
