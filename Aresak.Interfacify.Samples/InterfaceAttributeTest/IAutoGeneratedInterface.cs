﻿namespace Aresak.Interfacify.Samples.InterfaceAttributeTest;

/// <summary>
/// Sample interface for the <see cref="InterfacifyAttribute"/>.
/// </summary>
[Interfacify]
internal interface IAutoGeneratedInterface
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
}
