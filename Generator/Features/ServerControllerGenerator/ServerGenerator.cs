using Generator.Abstractions;
using Generator.Helpers;
using Uztelecom.Template.Generator.Helpers;

namespace Generator.Features.ServerControllerGenerator;

public class ServerGenerator: IGenerator
{
    
    public ServerGenerator(ServiceMetadata _type)
    {

        Type = _type;
        GenerateMethods = (string name) =>
        {
            var stringTemplate = "";

            foreach (var method in generatedMethodList)
            {
                var str = method.GenerateControllerBody(name);
                stringTemplate += $"{str} \n";
            }
            return stringTemplate;
        };
        ContrTempl = (string name) => @$"
        using Microsoft.AspNetCore.Mvc;
        using Stl.Fusion.Server;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Server.Controllers;

        [Route(""api/[controller]/[action]"")]
        [ApiController, JsonifyErrors]
        public class {name}Controller : ControllerBase, I{name}Service
        {{
            private readonly I{name}Service _{name.ToLower()};

            public {name}Controller(I{name}Service {name.ToLower()}) => _{name.ToLower()} = {name.ToLower()};

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
        var seviceName = Type.Name.Substring(1).Replace("Service", "");

        foreach (var method in Type.methodSymbols)
        {
            var methodGenerator = new MethodDeclaration(method);


            string[] strings = (from p in method.Parameters select p.Name!.ToString()).ToArray<string>();
            generatedMethodList.Add(new MethodType()
            {
                MethodDeclaration = methodGenerator.Generate(),
                HttpMethodType = HttpMethods.DefineHttpMethodByMethodName(method.Name),
                MethodName = method.Name,
                MethodParameters = string.Join(", ", strings)
            });
            controllersTemplates.Add(ContrTempl(seviceName));

        }

        return new GeneratorMetadata(controllersTemplates, @$"{seviceName}Controller");
    }
}

