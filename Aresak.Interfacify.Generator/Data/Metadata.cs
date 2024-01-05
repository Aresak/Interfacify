using Microsoft.CodeAnalysis;
using System;

namespace Aresak.Interfacify.Data;

/// <summary>
/// Base class for all metadata classes
/// </summary>
internal record Metadata
{
    /// <summary>
    /// Access modifier of the member
    /// </summary>
    public Accessibility Accessibility;

    /// <summary>
    /// Name of the member
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Type of the member stringified
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Can be thrown if accessiblity has not be translated to string</exception>
    public string AccessibilityToString()
    {
        // TODO: There are more access modifiers,
        // but I don't know how to use them yet.

        return Accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => throw new NotImplementedException($"Cannot convert accesibility '{Accessibility}' into string"),
        };
    }
}