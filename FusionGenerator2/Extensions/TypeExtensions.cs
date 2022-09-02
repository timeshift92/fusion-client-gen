using console.Pluralization;
using System.Collections.Immutable;
using System.Reflection;

namespace console;

public static class TypeExtensions
{

    public static bool IsList(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
    public static bool IsNullable(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
    public static bool IsTask(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
    }

    public static ImmutableArray<ClassMeta> ParseMeta(this List<Type> classes) => classes.Select(cls =>
    {
        var propertyInfos = cls.GetProperties().ToList();

        var propertyMetas = GetPropertyMetas(propertyInfos);

        return new ClassMeta()
        {
            Name = cls.Name,
            ServiceName = cls.Name.Replace("Entity", ""),
            PluralName = Pluralizer.Pluralize(cls.Name.Replace("Entity", "")),
            LowerPluralName = Pluralizer.Pluralize(cls.Name.Replace("Entity", "")).ToLower(),
            LowerName = cls.Name.Replace("Entity", "").ToLower(),
            Namespace = cls.Namespace!,
            Rootnamespace = cls.Namespace!.IndexOf(".") > 0 ? cls.Namespace.Split(".").First() : cls.Namespace,
            Primary = propertyMetas.First(x => x.IsPrimary),
            TypeParameters = propertyMetas
        };

    }).ToImmutableArray();


    public static ImmutableArray<PropertyMeta> GetPropertyMetas(List<PropertyInfo> properties)
    {
        var props = properties.Where(x => !(new string[] { "CreatedAt", "UpdatedAt" }.Contains(x.Name))).Select(x => new PropertyMeta()
        {
            Name = x.Name,
            IsPrimary = x.CustomAttributes.Any(x => x.ToString().Contains("KeyAttribute")),
            Type = GetPropertyType(x.PropertyType!.Name!, x.PropertyType)
        }).ToImmutableArray();
        return props;
    }

    public static string GetPropertyType(string TypeName, Type type)
    {
        if (type.IsGenericType)
        {
            return $"{type.Name.Replace("`1", "")}<{GetPropertyType(type.GenericTypeArguments[0].Name, type.GenericTypeArguments[0])}>";
        }
        return TypeName switch
        {
            "Int64" => "long",
            "Int32" => "int",
            "Double" => "double",
            "String" => "string",
            "Boolean" => "bool",
            "DateTime" => "DateTime",
            "DateTimeOffset" => "DateTimeOffset",
            "Task" => "Task",
            "Void" => "void",
            _ => TypeName

        };
    }

}