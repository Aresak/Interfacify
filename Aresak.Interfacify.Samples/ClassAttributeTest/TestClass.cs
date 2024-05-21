namespace Aresak.Interfacify.Samples.ClassAttributeTest;

/// <summary>
/// Sample class for the <see cref="InterfacifyAttribute"/>.
/// </summary>
[Interfacify(Template.NotifyPropertyChanged)]
internal partial class TestClass : ITestInterface
{
    // Id is not generated, because it is already defined in this class.
    public Guid Id { get; }

    // Missing interface members are generated.
}