using System;

namespace Aresak.Interfacify;

/// <summary>
/// Generate members for the class from all implemented interfaces.
/// Optionally specify a template to use.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InterfacifyAttribute : Attribute
{
    /// <summary>
    /// Template to use when generating the interface.
    /// </summary>
    public Template Template { get; }

    /// <summary>
    /// Generate members for the class from all implemented interfaces vy specified template.
    /// </summary>
    /// <param name="template">Template to use for generating the classes</param>
    public InterfacifyAttribute(Template template)
    {
        Template = template;
    }

    /// <summary>
    /// Generate members for the class from all implemented interfaces using the Basic template.
    /// </summary>
    public InterfacifyAttribute()
    {
        Template = Template.Basic;
    }
}
