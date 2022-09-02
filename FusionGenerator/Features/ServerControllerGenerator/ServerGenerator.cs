using FusionGenerator;
using FusionGenerator.Abstractions;
using FusionGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FusionGenerator.Features.ServerControllerGenerator;

public class ServerGenerator : IGenerator
{

    public ServerGenerator(ServiceMetadata _type)
    {

        Type = _type;
        GenerateMethods = (name) =>
        {
            var stringTemplate = "";

            foreach (var method in generatedMethodList)
            {
                var str = method.GenerateControllerBody(name);
                stringTemplate += $"{str} \n";
            }
            return stringTemplate;
        };
        ContrTempl = (name) => @$"
        using Microsoft.AspNetCore.Mvc;
        using Stl.Fusion.Server;
        using Stl.Fusion.Authentication;
        using {FusionServicesGenerator.RootNameSpace}.Shared;

        namespace {FusionServicesGenerator.RootNameSpace}.Server.Controllers;

        [Route(""api/[controller]/[action]"")]
        [ApiController, JsonifyErrors]
        public partial class {name}Controller : ControllerBase, I{name}Service
        {{
            private readonly I{name}Service _{name.ToLower()};
            private readonly ISessionResolver _sessionResolver;

            public {name}Controller(I{name}Service {name.ToLower()},ISessionResolver sessionResolver) {{
                _{name.ToLower()} = {name.ToLower()};
                _sessionResolver = sessionResolver;
            }} 

            {GenerateMethods(name)}
        }}";

    }

    private Func<string, string> ContrTempl { get; set; }
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
                IsSession = method.IsSessionCommand,
                MethodDeclaration = method.Generate(HttpMethods.DefineHttpMethodByMethodName(method.Name), true),
                HttpMethodType = HttpMethods.DefineHttpMethodByMethodName(method.Name),
                MethodName = method.Name,
                MethodParameters = string.Join(", ", method.Parameters.Select(x => x.Split(' ')[1]))
            });
            controllersTemplates.Add(ContrTempl(serviceName));

        }

        return new GeneratorMetadata(controllersTemplates, @$"{serviceName}Controller");
    }
}


