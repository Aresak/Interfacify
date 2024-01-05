# Interfacify
Automates the generation of interface-compliant properties based on attributes.

## Installation

Install the package from NuGet:

```
Install-Package Aresak.Interfacify
```

## Usage

Add using statement to the top of the file:

```csharp
using Aresak.Interfacify;
```

Design the interface, for our examples it looks like this:

```csharp
public interface IMyInterface
{
	string MyProperty { get; set; }

	string MyOtherProperty { get; set; }
}
```

Create a class that implements the interface and add `[Interfacify]` attribute:

```csharp
[Interfacify)]
public class MyClass : IMyInterface
{
	public string MyProperty { get; set; }
}
```

The following code will be generated:

```csharp

public class MyClass : IMyInterface
{
	public string MyProperty { get; set; }

	public string MyOtherProperty { get; set; }
}
```

### Specifying a generator template

In case there is needed to have a custom template for the generated code, 
it can be specified by using the `Template` parameter of the `InterfacifyAttribute`:

```csharp
[Interfacify(Template.NotifyPropertyChanged)]
public class MyClass : IMyInterface
{
// ...
```

### The following templates are available:

The code snippets below are generated examples.

#### 0. Basic

Default template. Creates a basic implementation of the interface.

With the specified rules:

- For property with only getter: `string MyProperty { get; }`
- For property with getter and setter: `string MyProperty { get; set; }`
- For property with neither getter or setter: `string MyProperty;`
- Access modifiers can be used

```csharp
[Interfacify(Template.Basic)]
public class MyClass : IMyInterface
{
	public string MyProperty { get; set; }

	public string MyOtherProperty { get; set; }
}
```

#### 1. NotifyPropertyChanged

Creates a basic implementation of the interface, but also implements `INotifyPropertyChanged` 
and raises the `PropertyChanged` event when a property is changed.

With the specified rules:

- For property with only getter: `string MyProperty { get; }`
- For other properties it has `{ get; set; }` and also raises the `PropertyChanged` event
- Access modifiers can be used

```csharp
[Interfacify(Template.NotifyPropertyChanged)]
public class MyClass : IMyInterface
{
	// Generated required code
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

	// Generated properties
	private string _generated_MyProperty;
	public string MyProperty { 
		get => _generated_MyProperty;
		set {
			_generated_MyProperty = value;
			NotifyPropertyChanged();
		}
	}

	private string _generated_MyOtherProperty;
	public string MyOtherProperty { 
		get => _generated_MyOtherProperty;
		set {
			_generated_MyOtherProperty = value;
			NotifyPropertyChanged();
		}
	}
}
```

## Common issues

### My IDE is giving me errors that my class does not implement the interface

If you are sure that you have assigned the attribute to the class correctly,
clean and rebuild the solution. If that does not help, try to restart the IDE.
In worst scenarios, the IDE must be restarted multiple times.