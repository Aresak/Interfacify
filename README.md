# Interfacify

Automates the generation of interface-compliant classes or missing properties in a class.

[![NuGet](https://img.shields.io/nuget/v/Aresak.Interfacify.svg)](https://www.nuget.org/packages/Aresak.Interfacify/)

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

### Auto-generated class from an interface

If you wish to automatically generate a class that implements the interface, you can use the `InterfacifyAttribute`:

```csharp
[Interfacify]
public interface IMyInterface
{
	string MyProperty { get; set; }

	string MyOtherProperty { get; set; }
}
```

The following code will be generated:

```csharp
public partial class MyInterface : IMyInterface
{
	public string MyProperty { get; set; }

	public string MyOtherProperty { get; set; }
}
```

It will create a new class with the same name as the interface, but without the `I` prefix.
In case, your interface doesn't begin with an `I`, the class will have a suffix `Class`.

Examples:

- The interface `IMyInterface` will generate a class `MyInterface`.
- The interface `MyInterface` will generate a class `MyInterfaceClass`.

### Generating missing properties to a class that implements the interface

If you wish to only auto-generate missing interface properties on a class, you can use the `InterfacifyAttribute`.
That way if you want to define custom property definitions, you can do so.

Create a class that implements the interface and add `[Interfacify]` attribute:

```csharp
[Interfacify)]
public partial class MyClass : IMyInterface
{
	public string MyProperty { get; set; }
}
```

The following code will be generated:

```csharp

public partial class MyClass : IMyInterface
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
public partial class MyClass : IMyInterface
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
public partial class MyClass : IMyInterface
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
public partial class MyClass : IMyInterface
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