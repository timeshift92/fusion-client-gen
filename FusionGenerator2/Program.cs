using console;
using FusionGenerator;
using FusionGenerator.Features.ClientDefinitionGenerator;
using FusionGenerator.Features.ServerControllerGenerator;
using System;
using System.Reflection;
using System.Threading.Tasks;
using static console.GlobalOptions;
using static console.TGenerate;
using static console.TypeExtensions;



foreach (var item in args)
{
    if (item.Equals("true"))
    {
        CanOverrideFile = true;
    }
}
try
{
#if DEBUG
    projectdir = Directory.GetParent(GetApplicationRoot(Directory.GetCurrentDirectory()))!.FullName;
    var DLL = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), "Shared.dll"));

    foreach (var item in DLL.GetReferencedAssemblies())
    {
        Assembly.Load(item);

    } 
#else
    var DLL = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net6.0/Shared.dll"));
    projectdir = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
#endif

    var classes = (from t in DLL.GetTypes()
                   where t.IsClass && t.Name.EndsWith("Entity")
                   select t).ToList();

    RootNamespace = classes.First()!
        .Namespace!.IndexOf(".") > 0 ?
        classes.First()!.Namespace!.Split(".").First()
        : classes.First()!.Namespace!;

    var metas = classes.ParseMeta();

    foreach (var meta in metas)
    {
        foreach (var item in meta.TypeParameters)
        {
            item.Metas = metas;
        }
    }

    DbContext(metas);
    FusionServerExtension(metas);
    Directory.CreateDirectory(CommandPath("t").Replace("tCommands.cs", ""));
    Directory.CreateDirectory(ServicePath("t").Replace("tService.cs", ""));
    Directory.CreateDirectory(InterfacePath("t").Replace("ItService.cs", ""));

    foreach (var meta in metas)
    {

        Command(meta);
        Interface(meta);
        Service(meta);

        var pathForFile = RazorPath(meta.ServiceName);
        Directory.CreateDirectory(pathForFile);
        ListRazor(meta, pathForFile);

        foreach (var relation in meta.Relations)
        {
            SelectRazor(metas.First(x => x.Name.Equals(relation.Entity)), pathForFile, relation.Type.Contains("List"));

        }

        FormRazor(meta, pathForFile);
        CreateFormRazor(meta, pathForFile);
        UpdateFormRazor(meta, pathForFile);

    }

    var interfaces = (from t in DLL.GetTypes()
                      where t.IsInterface && t.Name.EndsWith("Service")
                      select t).ToList();


    var serviceMetas = interfaces.Select(x => new ServiceMetadata()
    {
        Name = x.Name,
        MethodSymbols = x.GetMembers().Select((z) => new MethodMeta()
        {
            Name = z.Name,
            IsSessionCommand = (z as MethodInfo)!.GetParameters().First().ParameterType.GetTypeInfo().ImplementedInterfaces.Any(x => x.Name.Contains("ISession")),
            ReturnType = GetPropertyType((z as MethodInfo)!.ReturnType.Name, (z as MethodInfo)!.ReturnType),
            Arguments = (z as MethodInfo)!.GetParameters().Select(x => $"{GetPropertyType(x.ParameterType.Name, x.ParameterType)} {x.Name}").ToList(),
            Attributes = z.CustomAttributes.Select(x => x.AttributeType.Name).ToList(),
            ParentType = z.DeclaringType!.Name,
        }).ToList(),
    }).ToList();



    Directory.CreateDirectory(Path.Combine(Path.Combine(projectdir, "Client"), "Services"));
    GenerateTemplate<ClientGenerator>(serviceMetas, Path.Combine(projectdir, "Client"), "Services");
    Directory.CreateDirectory(Path.Combine(Path.Combine(projectdir, "Server"), "Controllers"));
    GenerateTemplate<ServerGenerator>(serviceMetas, Path.Combine(projectdir, "Server"), "Controllers");



    void GenerateTemplate<T>(List<ServiceMetadata> typeList, string projectDirectory, string generateDirectory) where T : class, IGenerator
    {
        string projectPath = Path.Combine(projectDirectory, generateDirectory);
        string listServices = "";
        foreach (var type in typeList)
        {
            var sg = (T)Activator.CreateInstance(typeof(T), type)!;
            var metadata = sg.Init();
            listServices += $"fusionClient.AddReplicaService<{type.Name}, {type!.Name!.Replace("Service", "Client")}Def>();\n";
            var template = "";
            foreach (string? item in metadata.Templates)
            {
                template = item.ToString();
            }
            string filename = $"{metadata.ControllerName}.g.cs";
            if (!CanOverrideFile && File.Exists(Path.Combine(projectPath, filename))) continue;
            File.WriteAllText(Path.Combine(projectPath, filename), $"{template}");
        }

        if (generateDirectory != "Services")
        {
            return;
        }

        string ExtensionTemplate(string clientDefinitions) =>
            @$"
using Stl.Fusion.Client;
using {RootNamespace}.Client.Services;
using {RootNamespace}.Shared;


namespace {RootNamespace}.Client;

public static class FusionClientExtension
{{
    public static FusionRestEaseClientBuilder AddFusionClients(this FusionRestEaseClientBuilder fusionClient)
    {{
        {clientDefinitions}

        return fusionClient;
    }}
}}";
        // if (!Options.CanOverrideFile && File.Exists(Path.Combine(projectDirectory, "FusionClientExtension.g.cs"))) return;
        File.WriteAllText(Path.Combine(projectDirectory, "FusionClientExtension.g.cs"), $"{ExtensionTemplate(string.Join("", listServices))}");


    }


    Console.WriteLine("Generation Success");

}
catch (Exception e)
{
    File.WriteAllText("error.log", e.Message);
    Console.WriteLine(e.Message);
    throw new Exception("failed");
}

