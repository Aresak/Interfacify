using Aresak.Interfacify.Attributes;

namespace Aresak.Interfacify.Samples;

[Interfacify(Template.NotifyPropertyChanged)]
internal partial class TestClass : ITestInterface
{
    public Guid Id { get; }
}
