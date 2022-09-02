using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using FusionGenerator.Features.ClientDefinitionGenerator;
using FusionGenerator.Features.ServerControllerGenerator;
using FusionGenerator.Abstractions;
using FusionGenerator.Helpers;
using FusionGenerator.Extensions;

namespace FusionGenerator;

[Generator]
public class FusionServicesGenerator : IIncrementalGenerator
{
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is InterfaceDeclarationSyntax cds &&
            cds.Parent!.ToString().Contains("Shared") &&
               cds.Identifier.ValueText.Contains("Service");
    }

    private static InterfaceDeclarationSyntax? InterfaceToAugment(GeneratorSyntaxContext context)
    {
        return (InterfaceDeclarationSyntax)context.Node;
    }
    public static string? RootNameSpace { get; set; }

    public static Compilation Compilation { get; set; } = null!;
    public static GlobalOptions Options { get; set; } = null!;
    public static void Execute(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, GlobalOptions opt, SourceProductionContext context)
    {
        Compilation = compilation;
        Options = opt;

        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        string? projectDirectory = opt.CurrentDirPath;

        try
        {

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "SC001",
                    "OK",
                    "FusionServicesGenerator ---- Generate File",
                    "OK",
                    DiagnosticSeverity.Info,
                    true), null, "", null));



            RootNameSpace = opt.GetSolutionFileName().Replace(".sln", "");
            var Methods = (InterfaceDeclarationSyntax service) =>
             {
                 var methodDeclarations = service!.Members!.Select(methods => methods as MethodDeclarationSyntax).ToList();
                 var mtd = new List<MethodMeta>();
                 foreach (var methodDeclaration in methodDeclarations)
                 {
                     mtd.Add(new MethodMeta(methodDeclaration!));
                 }
                 //var mt = service!.Members!.Select(methods => new MethodMeta((methods as MethodDeclarationSyntax)))!.ToList();
                 return mtd;
             };

            var typeList = classes.Select(service => new ServiceMetadata()
            {
                Name = service.Identifier.ToString(),
                MethodSymbols = Methods(service)
            }).ToList();


            Directory.CreateDirectory(Path.Combine(Path.Combine(projectDirectory, "Client"),"Services"));
            GenerateTemplate<ClientGenerator>(typeList, Path.Combine(projectDirectory, "Client"), "Services");
            Directory.CreateDirectory(Path.Combine(Path.Combine(projectDirectory, "Server"),"Controllers"));
            GenerateTemplate<ServerGenerator>(typeList, Path.Combine(projectDirectory, "Server"), "Controllers");
        }
        catch (Exception e)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "SG0001",
                    "Non-void method return type",
                    e.Message,
                    "yeet",
                    DiagnosticSeverity.Error,
                    true), null, "", null));
            return;
        }
        context.AddSource($"t.g.cs", "");
    }

    public static void GenerateTemplate<T>(List<ServiceMetadata> typeList, string projectDirectory, string generateDirectory) where T : class, IGenerator
    {
        string projectPath = Path.Combine(projectDirectory, generateDirectory);
        string listServices = "";
        foreach (var type in typeList)
        {
            var sg = (T)Activator.CreateInstance(typeof(T), type);
            var metadata = sg.Init();
            listServices += $"fusionClient.AddReplicaService<{type.Name}, {type!.Name!.Replace("Service", "Client")}Def>();\n";
            var template = "";
            foreach (string? item in metadata.Templates)
            {
                template = item.ToString();
            }
            string filename = $"{metadata.ControllerName}.g.cs";
            if (!Options.CanOverrideFile && File.Exists(Path.Combine(projectPath, filename))) continue;
            File.WriteAllText(Path.Combine(projectPath, filename), $"{SourceText.From(template, Encoding.UTF8)}");
        }

        if (generateDirectory != "Services")
        {
            return;
        }

        string ExtensionTemplate(string clientDefinitions) =>
            @$"
using Stl.Fusion.Client;
using {RootNameSpace}.Client.Services;
using {RootNameSpace}.Shared;
    

namespace {RootNameSpace}.Client;

public static class FusionClientExtension
{{
    public static FusionRestEaseClientBuilder AddFusionClients(this FusionRestEaseClientBuilder fusionClient)
    {{
        {clientDefinitions}

        return fusionClient;
    }}
}}";
        // if (!Options.CanOverrideFile && File.Exists(Path.Combine(projectDirectory, "FusionClientExtension.g.cs"))) return;
        File.WriteAllText(Path.Combine(projectDirectory, "FusionClientExtension.g.cs"), $"{SourceText.From(ExtensionTemplate(string.Join("", listServices)), Encoding.UTF8)}");


    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var globalOptions = context.AnalyzerConfigOptionsProvider.Select(GlobalOptions.Select);

        IncrementalValuesProvider<InterfaceDeclarationSyntax> declarations = context!.SyntaxProvider!
    .CreateSyntaxProvider(
        predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
        transform: static (ctx, _) => InterfaceToAugment(ctx))
    .Where(static m => m is not null)!;

        var compilationAndClasses = context.CompilationProvider
            .Combine(declarations.Collect())
            .Combine(globalOptions)
            ;

        globalOptions.Combine(compilationAndClasses);

        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Left.Left, source.Left.Right, source.Right, spc));

    }
}

//#if DEBUG
//        if (!Debugger.IsAttached)
//        {
//            Debugger.Launch();
//        }
//#endif
