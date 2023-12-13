using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Aresak.Interfacify.Generator.Extensions;

internal static class AttributeDataExtensions
{
    internal static T MapToType<T>(this AttributeData attributeData) where T : Attribute
    {
        T attribute = Instantiate<T>();

        foreach (KeyValuePair<string, TypedConstant> arguments in attributeData.NamedArguments)
        {
            typeof(T).GetField(arguments.Key).SetValue(attribute, arguments.Value.Value);
        }

        return attribute;
    }

    static T Instantiate<T>()
    {
        T attribute = (T)Activator.CreateInstance(typeof(T));
        return attribute;
    }
}
