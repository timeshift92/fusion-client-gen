using console.Extensions;
using static console.GlobalOptions;


namespace console;

public partial class TGenerate
{
    public static void Command(ClassMeta meta)
    {
        if (!CanOverrideFile && File.Exists(CommandPath(meta.ServiceName))) return;

        File.WriteAllText(CommandPath(meta.ServiceName), TemplateGenerator.Execute(
            ResourceReader.GetResource("Command.txt"), meta));
    }
}