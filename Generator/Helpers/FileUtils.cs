using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Uztelecom.Template.Generator.Helpers;

public static class FileUtils
{


    public static string GetAssemblyFileName() => GetAssemblyPath().Split('\\').Last();
    public static string GetAssemblyDir() => Path.GetDirectoryName(GetAssemblyPath());
    public static string GetAssemblyPath() => Assembly.GetExecutingAssembly().Location;
    public static string GetSolutionFileName() => GetSolutionPath().Split('\\').Last();
    public static string GetSolutionDir() => Directory.GetParent(GetSolutionPath()).FullName;
    public static string CurrentDirPath = GetAssemblyDir();
    public static string GetSolutionPath()
    {
        while (CurrentDirPath != null)
        {
            var fileInCurrentDir = Directory.GetFiles(CurrentDirPath).Select(f => f.Split('\\').Last()).ToArray();
            var solutionFileName = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase));
            if (solutionFileName != null)
                return Path.Combine(CurrentDirPath, solutionFileName);

            CurrentDirPath = Directory.GetParent(CurrentDirPath)?.FullName;
        }

        throw new FileNotFoundException("Cannot find solution file path");
    }
}
