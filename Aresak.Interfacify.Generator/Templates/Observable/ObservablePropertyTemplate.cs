using Aresak.Interfacify.Generator.Data;

namespace Aresak.Interfacify.Generator.Templates.Observable;

internal class ObservablePropertyTemplate(PropertyMetadata property) : PropertyTemplate(property)
{
    string privateValueName => $"_generated_{Property.Name}";

    string privateValueDeclaration => $"{Property.AccessibilityToString()} {Property.Type.Name} {privateValueName}";
    string publicValueDeclaration => $"{Property.AccessibilityToString()} {Property.Type.Name} {Property.Name}";

    protected override string GenerateEmpty()
    {
        return GenerateWithGetterSetter();
    }

    protected override string GenerateWithGetterOnly()
    {
        return $@"
        {publicValueDeclaration} {{
            get;
        }}
        ";
    }

    protected override string GenerateWithGetterSetter()
    {
        string declaration = GeneratePropertyDeclaration();

        return $@"
        {declaration} {{
            get => {privateValueName};
            set
            {{
                if (value != {privateValueName})
                {{
                    {privateValueName} = value;
                    NotifyPropertyChanged();
                }}
            }}
        }}
        ";
    }

    protected override string GeneratePropertyDeclaration()
    {
        return $@"
        {privateValueDeclaration};
        {publicValueDeclaration}
        ";
    }
}
