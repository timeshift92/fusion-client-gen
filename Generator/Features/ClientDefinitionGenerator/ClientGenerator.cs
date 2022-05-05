using Generator.Abstractions;
using Generator.Helpers;
using Uztelecom.Template.Generator.Helpers;

namespace Generator.Features.ClientDefinitionGenerator;

public class ClientGenerator : IGenerator
{
    public ClientGenerator(ServiceMetadata _type)
    {
        Type = _type;
        GenerateMethods = (string name) =>
        {
            var stringTemplate = "";

            foreach (var method in generatedMethodList)
            {
                var str = method.GenerateClientBody();
                stringTemplate += $"{str} \n";
            }
            return stringTemplate;
        };
        ContrTempl = (string name) => @$"
        using RestEase;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Client.Services;

        [BasePath(""{name}"")]
        public interface I{name}ClientDef
        {{
        {GenerateMethods(name)}
        }}
        ";

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
            var methodGenerator = new MethodDeclaration(method, true);

            string[] strings = (from p in method.Parameters select p.Name.ToString()).ToArray<string>();

            generatedMethodList.Add(new MethodType()
            {
                MethodDeclaration = methodGenerator.Generate(),
                HttpMethodType = HttpMethods.DefineHttpMethodByMethodName(method.Name),
                MethodName = method.Name,
                MethodParameters = String.Join(", ", strings)


            });
            controllersTemplates.Add(ContrTempl(Type.Name.Substring(1).Replace("Service", "")));

        }

        return new GeneratorMetadata(controllersTemplates, @$"{seviceName}Client");
    }
}

