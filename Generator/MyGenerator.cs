using System.Diagnostics;
using System.Reflection;
using System.Text;
using Generator.Abstractions;
using Generator.Features.ClientDefinitionGenerator;
using Generator.Features.ServerControllerGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Uztelecom.Template.Generator.Helpers;

namespace Generator;

[Generator]
public class HelloWorldGenerator : ISourceGenerator
{
    public string RootNameSpace { get; set; }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectDirectory", out var projectDirectory) == false)
        {
            throw new ArgumentException("MSBuildProjectDirectory");
        }

        FileUtils.CurrentDirPath = projectDirectory;
        RootNameSpace = FileUtils.GetSolutionFileName().Replace(".sln", "");

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
      
        var tps = context.Compilation.SourceModule.ReferencedAssemblySymbols.Where(x => x.Name.Contains("Shared")).First().GlobalNamespace;

        foreach (var space in RootNameSpace.Split('.'))
        {
            tps = tps.GetNamespaceMembers().First(n => n.Name == space);
        }
        var tps1 = tps.GetNamespaceMembers().First(n => n.Name == "Shared")
            .GetTypeMembers()
            .Where(x => x.Name.Contains("Service"))
            ;

        var typeList = tps1.Select(service => new ServiceMetadata()
        {
            Name = service.Name,
            methodSymbols = service.GetMembers().Select(methods => methods.OriginalDefinition as IMethodSymbol).ToList<IMethodSymbol>()
        }).ToList();



        if (projectDirectory.ToLower().IndexOf("server") != -1)
        {
            GenerateTemplate<ServerGenerator>(typeList, projectDirectory, "Controllers");

        }
        else if (projectDirectory.ToLower().IndexOf("client") != -1)
        {
            GenerateTemplate<ClientGenerator>(typeList, projectDirectory, "Services");
        }

    }

    public void GenerateTemplate<T>(List<ServiceMetadata> typeList, string projectDirectory, string generateDirectory) where T : class, IGenerator
    {
        var projectPath = Path.Combine(projectDirectory, generateDirectory);
        var listServices = "";
        foreach (var type in typeList)
        {
            var sg = (T)Activator.CreateInstance(typeof(T), type);
            var metadata = sg.Init();
            listServices += $"fusionClient.AddReplicaService<{type.Name}, {type.Name.Replace("Service", "Client")}Def>();\n";
            foreach (var item in metadata.Templates)
            {
                var filename = $"{metadata.ControllerName}.g.cs";
                File.WriteAllText(Path.Combine(projectPath, filename), $"{SourceText.From(item, Encoding.UTF8)}");
            }
        }
        if (generateDirectory == "Services")
        {
            var extensionTempl = (string clientDefs) => @$"
using Stl.Fusion.Client;
using {RootNameSpace}.Client.Services;
using {RootNameSpace}.Shared;
    

namespace Uztelecom.Template.Client;

public static class FusionClientExtension
{{
    public static FusionRestEaseClientBuilder AddFusionClients(this FusionRestEaseClientBuilder fusionClient)
    {{
        {clientDefs}

        return fusionClient;
    }}
}}";
            File.WriteAllText(Path.Combine(projectDirectory, "FusionClientExtension.g.cs"), $"{SourceText.From(extensionTempl(string.Join("", listServices)), Encoding.UTF8)}");
        }


    }


    public void Initialize(GeneratorInitializationContext context)
    {


    }


}

//#if DEBUG
//        if (!Debugger.IsAttached)
//        {
//            Debugger.Launch();
//        }
//#endif