#nullable disable

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aresak.Interfacify.Templates.Observable;

[GeneratedCode("Interfacify", "Example")]
internal class ObservableClassExample : INotifyPropertyChanged
{
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

    private string name;
    public string Name
    {
        get => name;
        set
        {
            if (value != name)
            {
                name = value;
                NotifyPropertyChanged();
            }
        }
    }
}
