using Aresak.Interfacify.Data;

namespace Aresak.Interfacify.Templates.Observable;

internal class ObservablePropertyTemplate(PropertyMetadata property) : PropertyTemplate(property)
{
    string privateValueName => $"_generated_{Property.Name}";

    string privateValueDeclaration => $"{Property.AccessibilityToString()} {Property.Type.Name} {privateValueName}";
    string publicValueDeclaration => $"{Property.AccessibilityToString()} {Property.Type.Name} {Property.Name}";

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

    protected override string GenerateEmpty()
    {
        // Even empty properties need to have getter and setter
        // for PropertyChanged to work.
        return GenerateWithGetterSetter();
    }

    protected override string GeneratePropertyDeclaration()
    {
        return $@"
        {privateValueDeclaration};
        {publicValueDeclaration}
        ";
    }
}
