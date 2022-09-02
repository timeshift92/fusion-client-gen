using console.Extensions;
using static console.GlobalOptions;

namespace console;

public partial class TGenerate
{
    public static void Service(ClassMeta meta)
    {

        var path = ServicePath(meta.ServiceName);
        if (!CanOverrideFile && File.Exists(path)) return;

        File.WriteAllText(path, TemplateGenerator.Execute(
           ResourceReader.GetResource("Service.txt"), meta));
    }
}