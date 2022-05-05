using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Generator.Helpers;

public class MethodDeclaration
{
    private IMethodSymbol Method { get; }
    public bool IsClient { get; }
    private string ReturnType { get; set; }

    public MethodDeclaration(IMethodSymbol method, bool isClient = false)
    {
        Method = method;
        IsClient = isClient;
        ReturnType = method.ReturnType.ToString();
    }

    public string CutOrReplaceSystemTypes(string inlineTypesString)
    {
        var typeArr = inlineTypesString.Split(',');
        for (int i = 0; i < typeArr.Length; i++)
        {
            typeArr[i] = typeArr[i] switch
            {
                string type when type.Equals("System.Int16") => "short",
                string type when type.Equals("System.UInt16") => "ushort",
                string type when type.Equals("System.Int32") => "int",
                string type when type.Equals("System.UInt32") => "uint",
                string type when type.Equals("System.IntPtr") => "nint",
                string type when type.Equals("System.UIntPtr") => "nuint",
                string type when type.Equals("System.Int64") => "long",
                string type when type.Equals("System.UInt64") => "ulong",
                string type when type.Equals("System.Single") => "float",
                string type when type.Equals("System.String") => "string",
                string type when type.Equals("System.Double") => "double",
                string type when type.Equals("System.SByte") => "sbyte",
                string type when type.Equals("System.Byte") => "byte",
                _ => typeArr[i]
            };

            if (typeArr[i].ToLower().IndexOf("system") != -1)
            {
                typeArr[i] = typeArr[i].Substring(typeArr[i].LastIndexOf(".") + 1);
            }

        }
        return string.Join(", ", typeArr);

    }

    public string GenerateCascadeType()
    {
        var methodTypes = ReturnType.Split('`');

        var collapsableType = CutOrReplaceSystemTypes(methodTypes[0]);
        /*
         * we select in group all string between [ - - ] symbols 
         */
        var regexpResult = Regex.Match(methodTypes[1], @"\[(.+[\]\[]?)\]");
        var inlineTypes = CutOrReplaceSystemTypes(regexpResult.Groups[1].Value);

        var paramStrings = new List<string>();



        foreach (var attr in Method.Parameters)
        {
            if (IsClient && attr.HasExplicitDefaultValue)
            {
                paramStrings.Add($"{CutOrReplaceSystemTypes($"{attr} {attr.Name}")} = default");
            }
            else
            {
                paramStrings.Add(CutOrReplaceSystemTypes($"{attr} {attr.Name}"));
            }

        }
        return $"{collapsableType}<{inlineTypes}> {Method.Name}({String.Join(", ", paramStrings)} )";

    }


    public string Generate()
    {
        var paramStrings = new List<string>();

        foreach (var attr in Method.Parameters)
        {
            if (IsClient && attr.HasExplicitDefaultValue)
            {
                paramStrings.Add($"{CutOrReplaceSystemTypes($"{attr} {attr.Name}")} = default");
            }
            else
            {
                paramStrings.Add(CutOrReplaceSystemTypes($"{attr} {attr.Name}"));
            }

        }
        return $"{ReturnType} {Method.Name}({String.Join(", ", paramStrings)} )";
    }
}





//public class MethodDeclaration
//{
//    private IMethodSymbol Method { get; }
//    public bool IsClient { get; }
//    private string ReturnType { get; set; }

//    public MethodDeclaration(IMethodSymbol method, bool isClient = false)
//    {
//        Method = method;
//        IsClient = isClient;
//        ReturnType = method.ReturnType.ToString();
//    }

//    public string CutOrReplaceSystemTypes(string inlineTypesString)
//    {
//        var typeArr = inlineTypesString.Split(',');
//        for (int i = 0; i < typeArr.Length; i++)
//        {
//            typeArr[i] = typeArr[i] switch
//            {
//                string type when type.Equals("System.Int16") => "short",
//                string type when type.Equals("System.UInt16") => "ushort",
//                string type when type.Equals("System.Int32") => "int",
//                string type when type.Equals("System.UInt32") => "uint",
//                string type when type.Equals("System.IntPtr") => "nint",
//                string type when type.Equals("System.UIntPtr") => "nuint",
//                string type when type.Equals("System.Int64") => "long",
//                string type when type.Equals("System.UInt64") => "ulong",
//                string type when type.Equals("System.Single") => "float",
//                string type when type.Equals("System.String") => "string",
//                string type when type.Equals("System.Double") => "double",
//                string type when type.Equals("System.SByte") => "sbyte",
//                string type when type.Equals("System.Byte") => "byte",
//                _ => typeArr[i]
//            };

//            if (typeArr[i].ToLower().IndexOf("system") != -1)
//            {
//                typeArr[i] = typeArr[i].Substring(typeArr[i].LastIndexOf(".") + 1);
//            }

//        }
//        return string.Join(", ", typeArr);

//    }

//    public string GenerateCascadeType()
//    {
//        var methodTypes = ReturnType.Split('`');

//        var collapsableType = CutOrReplaceSystemTypes(methodTypes[0]);
//        /*
//         * we select in group all string between [ - - ] symbols 
//         */
//        var regexpResult = Regex.Match(methodTypes[1], @"\[(.+[\]\[]?)\]");
//        var inlineTypes = CutOrReplaceSystemTypes(regexpResult.Groups[1].Value);

//        var paramStrings = new List<string>();



//        foreach (var attr in Method.Parameters)
//        {
//            if (IsClient && attr.HasExplicitDefaultValue)
//            {
//                paramStrings.Add($"{CutOrReplaceSystemTypes($"{attr} {attr.Name}")} = default");
//            }
//            else
//            {
//                paramStrings.Add(CutOrReplaceSystemTypes($"{attr} {attr.Name}"));
//            }

//        }
//        return $"{collapsableType}<{inlineTypes}> {Method.Name}({String.Join(", ", paramStrings)} )";

//    }
//    public string GenarateSimpleType()
//    {
//        var paramStrings = new List<string>();

//        foreach (var attr in Method.Parameters)
//        {
//            paramStrings.Add(CutOrReplaceSystemTypes($"{attr} {attr.Name}"));
//        }
//        return $"{CutOrReplaceSystemTypes(ReturnType)} {Method.Name}({String.Join(", ", paramStrings)})";
//    }

//    public string Generate() => ReturnType switch
//    {
//        string type when type.LastIndexOf('`') > 0 => GenerateCascadeType(),
//        _ => GenarateSimpleType(),
//    };
//}
