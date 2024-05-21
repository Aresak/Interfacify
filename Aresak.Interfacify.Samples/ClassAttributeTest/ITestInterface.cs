namespace Aresak.Interfacify.Samples.ClassAttributeTest;

/// <summary>
/// Sample interface for the <see cref="InterfacifyAttribute"/>.
/// </summary>
internal interface ITestInterface
{
    Guid Id { get; }

    string Email { get; set; }

    string Password { get; set; }

    public string PublicString { get; set; }

    // Protected will break it
    //protected string ProtectedString { get; set; }

    int GetterOnly { get; }

    int SetterOnly { set; }

    int DiffGetter { get; protected set; }

    TestEnum Enum { get; set; }

    Guid SomeGuid { get; set; }

    DateTime SomeDateTime { get; set; }
}

internal enum TestEnum
{
    One,

    Two,

    Three
}