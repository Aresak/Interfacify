using Aresak.Interfacify.Data;

namespace Aresak.Interfacify.Templates.Observable;

internal class ObservableFileTemplate(ClassMetadata metadata) : FileTemplate(metadata)
{
    protected override string GenerateProperty(PropertyMetadata property)
    {
        ObservablePropertyTemplate template = new(property);
        return template.Generate();
    }

    protected override string AddUsingStatements()
    {
        return $@"
        using System.ComponentModel;
        using System.Runtime.CompilerServices;
        ";
    }

    protected override string AddAdditionalClassCode()
    {
        return $@"
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = """")
        {{
            if (PropertyChanged == null)
            {{
                return;
            }}

            PropertyChangedEventArgs arguments = new PropertyChangedEventArgs(propertyName);
            PropertyChanged.Invoke(this, arguments);
        }}
        ";
    }
}
