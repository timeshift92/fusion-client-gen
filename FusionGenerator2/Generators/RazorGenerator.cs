using console.Extensions;
using System.IO;
using static console.GlobalOptions;

namespace console;

public partial class TGenerate
{
    public static void ListRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{sep}{meta.ServiceName}List.razor";
        if (!CanOverrideFile && File.Exists(path)) return;

        var templateString = ResourceReader.GetResource("List.txt");
        var result = TemplateGenerator.Execute(templateString, meta);
        File.WriteAllText(path, result);
    }

    public static void SelectRazor(ClassMeta meta, string pathForFile, bool isList)
    {
        var type = isList ? "MultiSelect" : "Select";
        var path = $"{pathForFile}{sep}_{meta.Name}{type}.razor";
        if (!CanOverrideFile && File.Exists(path)) return;

        try
        {
            var templateString = ResourceReader.GetResource(isList ? "MultiSel.txt" : "Select.txt");
            var result = TemplateGenerator.Execute(templateString, meta);
            File.WriteAllText(path, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public static void FormRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{sep}_Form.razor";
        if (!CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Form.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText($"{pathForFile}{sep}_Form.razor", formResult);
    }

    public static void CreateFormRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{sep}Create.razor";
        if (!CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Create.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText(path, formResult);
    }
    public static void UpdateFormRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{sep}Update.razor";
        if (!CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Update.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText(path, formResult);
    }
}