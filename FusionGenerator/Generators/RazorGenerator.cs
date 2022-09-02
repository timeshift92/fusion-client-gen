using FusionGenerator.Extensions;
using FusionGenerator.Models;
using System.IO;

namespace FusionGenerator.Generators;

public partial class TGenerate
{
    public static void ListRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{GlobalOptions.sep}{meta.ServiceName}List.razor";
        if (!opt.CanOverrideFile && File.Exists(path)) return;

        var templateString = ResourceReader.GetResource("List.txt");
        var result = TemplateGenerator.Execute(templateString, meta);
        File.WriteAllText(path, result);
    }

    public static void SelectRazor(ClassMeta meta, string pathForFile,bool isList)
    {
        var type = isList ? "MultiSelect" : "Select";
        var path = $"{pathForFile}{GlobalOptions.sep}_{meta.Name}{type}.razor";
        if (!opt.CanOverrideFile && File.Exists(path)) return;

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
        var path = $"{pathForFile}{GlobalOptions.sep}_Form.razor";
        if (!opt.CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Form.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText($"{pathForFile}{GlobalOptions.sep}_Form.razor", formResult);
    }

    public static void CreateFormRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{GlobalOptions.sep}Create.razor";
        if (!opt.CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Create.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText(path, formResult);
    }
    public static void UpdateFormRazor(ClassMeta meta, string pathForFile)
    {
        var path = $"{pathForFile}{GlobalOptions.sep}Update.razor";
        if (!opt.CanOverrideFile && File.Exists(path)) return;

        var formString = ResourceReader.GetResource("Update.txt");
        var formResult = TemplateGenerator.Execute(formString, meta);
        File.WriteAllText(path, formResult);
    }
}