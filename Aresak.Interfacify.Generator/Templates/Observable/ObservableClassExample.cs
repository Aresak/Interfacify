// Example how the generated code will look like.

#nullable disable

using System.CodeDom.Compiler;

// <AddUsingStatements>
using System.ComponentModel;
using System.Runtime.CompilerServices;
// </ AddUsingStatements>

namespace Aresak.Interfacify.Templates.Observable;

[GeneratedCode("Interfacify", "Example")]
// <AddClassAttributes />
internal class ObservableClassExample : INotifyPropertyChanged
{
    // <AddAdditionalClassCode>
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChangedEventArgs arguments = new PropertyChangedEventArgs(propertyName);
        PropertyChanged.Invoke(this, arguments);
    }
    // </AddAdditionalClassCode>

    // <GenerateProperties>
    private string _generated_Name;
    public string Name
    {
        get => _generated_Name;
        set
        {
            if (value != _generated_Name)
            {
                _generated_Name = value;
                NotifyPropertyChanged();
            }
        }
    }
    // </GenerateProperties>
}
