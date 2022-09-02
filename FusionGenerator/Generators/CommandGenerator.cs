using FusionGenerator.Extensions;
using FusionGenerator.Models;
using System.Text;
namespace FusionGenerator.Generators;

public partial class TGenerate
{
    public static void Command(ClassMeta meta)
    {
        if (!opt.CanOverrideFile && File.Exists(opt.CommandPath(meta.ServiceName))) return;

        File.WriteAllText(opt.CommandPath(meta.ServiceName), TemplateGenerator.Execute(
            ResourceReader.GetResource("Command.txt"), meta));
    }
}