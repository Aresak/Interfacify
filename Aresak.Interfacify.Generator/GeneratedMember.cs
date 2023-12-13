using Microsoft.CodeAnalysis;

namespace Aresak.Interfacify.Generator;

internal class GeneratedMember(IPropertySymbol member)
{
    public string Name { get; set; } = member.Name;

    public Accessibility Accessibility { get; set; } = member.DeclaredAccessibility;

    public string Type { get; set; } = member.Type.Name;

    public string? Getter { get; set; }

    public string? Setter { get; set; }

    public override string ToString()
    {
        return $"{Accessibility} {Name} [G: {Getter != null} S: {Setter != null}]";
    }
}
