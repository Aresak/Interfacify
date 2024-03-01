namespace Aresak.Interfacify.Samples;

internal class Program
{
    static void Main(string[] args)
    {
        // This is just to make sure that the generated code compiles.
        TestClass testClass = new();

        Console.WriteLine($"Generated enum value: {testClass.Enum}");
        testClass.Enum = TestEnum.Three;
        Console.WriteLine($"Set enum value: {testClass.Enum}");

        // Wow, it works!
        Console.WriteLine("Hello, World!");
    }
}
