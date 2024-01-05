namespace Aresak.Interfacify;

/// <summary>
/// Template to use when generating the interface.
/// Default is Basic.
/// </summary>
public enum Template
{
    /// <summary>
    /// Uses the basic { get; set; } template.
    /// </summary>
    Basic,

    /// <summary>
    /// Adds a NotifyPropertyChanged event to the class.
    /// </summary>
    NotifyPropertyChanged
}
