
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using static FusionGenerator.Generators.TGenerate;
using static FusionGenerator.Extensions.RoslynExtensions;
using System.Diagnostics;
using FusionGenerator;
using FusionGenerator.Extensions;

namespace FusionGenerator;

public static class GeneratorPaths
{
    public static string CommandPath => "\\Shared\\Commands\\Post";
}


[Generator]
public class BlazorGenerator : IIncrementalGenerator
{

    public void Execute(Compilation compilation, ImmutableArray<RecordDeclarationSyntax> classes, GlobalOptions opt, SourceProductionContext context)
    {

        if (!Debugger.IsAttached && opt.IsDebug)
        {
            Debugger.Launch();
        }

        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var metas = ParseMeta(classes, opt);
        FusionGenerator.Generators.TGenerate.opt = opt;


        DbContext(metas);
        FusionServerExtension(metas);
        Directory.CreateDirectory(opt.CommandPath("t").Replace("tCommands.cs",""));
        Directory.CreateDirectory(opt.ServicePath("t").Replace("tService.cs",""));
        Directory.CreateDirectory(opt.InterfacePath("t").Replace("ItService.cs",""));

        foreach (var meta in metas)
        {
            foreach (var param in meta.TypeParameters)
            {
                param.Metas = metas;
            }
        }

        foreach (var meta in metas)
        {

            Command(meta);
            Interface(meta);
            Service(meta);

            var pathForFile = opt.RazorPath(meta.ServiceName);
            Directory.CreateDirectory(pathForFile);
            ListRazor(meta, pathForFile);

            foreach (var relation in meta.Relations)
            {
                SelectRazor(metas.First(x=>x.Name.Equals(relation.Entity)),pathForFile,relation.Type.Contains("List"));
                
            }

            FormRazor(meta, pathForFile);
            CreateFormRazor(meta, pathForFile);
            UpdateFormRazor(meta, pathForFile);

        }

    }
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {


        var globalOptions = context.AnalyzerConfigOptionsProvider.Select(GlobalOptions.Select);
        IncrementalValuesProvider<RecordDeclarationSyntax> declarations = context!.SyntaxProvider!
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => InterfaceToAugment(ctx))
            .Where(static m => m is not null)!;

        var compilationAndClasses = context.CompilationProvider
            .Combine(declarations.Collect())
            .Combine(globalOptions)
            ;

        context.RegisterSourceOutput(compilationAndClasses,
             (spc, source) => Execute(source.Left.Left, source.Left.Right, source.Right, spc));
    }


    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is RecordDeclarationSyntax cds &&
               cds.Identifier.ValueText.Contains("Entity");
    }

    private static RecordDeclarationSyntax? InterfaceToAugment(GeneratorSyntaxContext context)
    {

        return (RecordDeclarationSyntax)context.Node;
    }

    //#if DEBUG
    //        if (!Debugger.IsAttached)
    //        {
    //            Debugger.Launch();
    //        }
    //#endif
}