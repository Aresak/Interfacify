using Aresak.Interfacify.Generator.Attributes;

namespace Aresak.Interfacify.Samples;

[Interfacify(Generator.Template.NotifyPropertyChanged)]
internal partial class TestClass : ITestInterface
{
    public Guid Id { get; }
}
