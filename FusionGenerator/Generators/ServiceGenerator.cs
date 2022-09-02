using FusionGenerator.Extensions;
using FusionGenerator.Models;
using System.Diagnostics;

namespace FusionGenerator.Generators;

public partial class TGenerate
{
    public static void Service(ClassMeta meta)
    {
  
        var path = opt.ServicePath(meta.ServiceName);
        if (!opt.CanOverrideFile && File.Exists(path)) return;

        File.WriteAllText(path, TemplateGenerator.Execute(
           ResourceReader.GetResource("Service.txt"), meta));
    }
}