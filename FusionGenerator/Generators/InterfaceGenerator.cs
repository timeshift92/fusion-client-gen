using System.Diagnostics;
using FusionGenerator.Extensions;
using FusionGenerator.Models;

namespace FusionGenerator.Generators;

public partial class TGenerate
{
    public static GlobalOptions opt = null!;
    public static void Interface(ClassMeta meta)
    {
        var pathForInterface = opt.InterfacePath(meta.Name);

        if(!opt.CanOverrideFile && File.Exists(pathForInterface)) return;
       

        File.WriteAllText(pathForInterface, TemplateGenerator.Execute(
            ResourceReader.GetResource("Interface.txt"), meta));
    }
}