using System.Diagnostics;
using console.Extensions;
using static console.GlobalOptions;

namespace console;

public partial class TGenerate
{

    public static void Interface(ClassMeta meta)
    {
        var pathForInterface = InterfacePath(meta.Name);

        if (!CanOverrideFile && File.Exists(pathForInterface)) return;


        File.WriteAllText(pathForInterface, TemplateGenerator.Execute(
            ResourceReader.GetResource("Interface.txt"), meta));
    }
}