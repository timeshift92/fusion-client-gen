using System.Collections.Immutable;
using System.Text;
using FusionGenerator.Models;

namespace FusionGenerator.Generators;

public partial class TGenerate
{
    public static void FusionServerExtension(ImmutableArray<ClassMeta> metas)
    {

        // if (!opt.CanOverrideFile && File.Exists(opt.FusionServerExtension)) return;
        
        StringBuilder ex = new();
        ex.AppendLine($@"using {opt.RootNamespace}.Services;");
        ex.AppendLine($@"using {opt.RootNamespace}.Shared;");
        ex.AppendLine($@"using Stl.Fusion;");
        ex.AppendLine();
        ex.AppendLine($@"namespace {opt.RootNamespace}.Server;");
        ex.AppendLine($@"public  static class FusionServerExtension");
        ex.AppendLine($@"{{");
        ex.AppendLine($@"     public  static  FusionBuilder AddUtcServices(this FusionBuilder fusion)");
        ex.AppendLine($@"     {{");
        foreach (var meta in metas)
        {
            ex.AppendLine($@"       fusion.AddComputeService<I{meta.Name}Service,{meta.ServiceName}Service>();");

        }
        ex.AppendLine($@"        return fusion;");
        ex.AppendLine($@"     }}");
        ex.AppendLine($@"}}");


        File.WriteAllText(opt.FusionServerExtension, ex.ToString());
    }
}