using Microsoft.CodeAnalysis;
using System;

namespace Aresak.Interfacify.Data;

internal record Metadata
{
    public Accessibility Accessibility;
    public string Name = string.Empty;

    public string AccessibilityToString()
    {
        return Accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            //Accessibility.ProtectedOrInternal => "protected internal",
            //Accessibility.ProtectedAndInternal => "private protected",
            _ => throw new NotImplementedException($"Cannot convert accesibility '{Accessibility}' into string"),
        };
    }
}