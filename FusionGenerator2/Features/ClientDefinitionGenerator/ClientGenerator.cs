using console;
using FusionGenerator;
using System;
using System.Collections.Generic;

namespace FusionGenerator.Features.ClientDefinitionGenerator;

public class ClientGenerator : IGenerator
{
    public ClientGenerator(ServiceMetadata _type)
    {
        Type = _type;
        GenerateMethods = (name) =>
        {
            var stringTemplate = "";

            foreach (var method in generatedMethodList)
            {
                var str = method.GenerateClientBody();
                stringTemplate += $"{str} \n";
            }
            return stringTemplate;
        };
        ControllerTemplate = (name) => @$"
        using RestEase;
        using {GlobalOptions.RootNamespace}.Shared;
        using Stl.Fusion.Authentication;

        namespace {GlobalOptions.RootNamespace}.Client.Services;

        [BasePath(""{name}"")]
        [SerializationMethods(Query = QuerySerializationMethod.Serialized)]
        public interface I{name}ClientDef
        {{
        {GenerateMethods(name)}
        }}
        ";

    }

    private Func<string, string> ControllerTemplate { get; set; }
    private Func<string, string> GenerateMethods { get; set; }
    public ServiceMetadata Type { get; }

    private readonly List<MethodType> generatedMethodList = new();

    public GeneratorMetadata Init()
    {
        var controllersTemplates = new List<string>();
        string serviceName = Type!.Name!.Substring(1).Replace("Service", "");

        foreach (var method in Type!.MethodSymbols!)
        {

            generatedMethodList.Add(new MethodType()
            {
                MethodDeclaration = method.Generate(HttpMethods.DefineHttpMethodByMethodName(method.Name)),
                HttpMethodType = HttpMethods.DefineHttpMethodByMethodName(method.Name),
                MethodName = method.Name,
                MethodParameters = string.Join(", ", method!.Arguments)


            });
            controllersTemplates.Add(ControllerTemplate(Type.Name.Substring(1).Replace("Service", "")));

        }

        return new GeneratorMetadata(controllersTemplates, @$"{serviceName}Client");
    }
}
