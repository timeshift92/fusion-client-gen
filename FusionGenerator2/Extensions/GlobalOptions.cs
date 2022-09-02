using System.Reflection;
using System.Text.RegularExpressions;

namespace console;

internal static class GlobalOptions
{
    public static string RootNamespace = "";
    public static string projectdir = "";
    public static char sep = Path.DirectorySeparatorChar;
    public static string CommandPath(string serviceName) => $"{projectdir}{sep}Shared{sep}Commands{sep}{serviceName}Commands.cs";
    public static string DbContextPath => $"{projectdir}{sep}Services{sep}AppDbContext.g.cs";
    public static string FusionServerExtensionPath => $"{projectdir}{sep}Server{sep}FusionServerExtension.cs";
    public static string InterfacePath(string serviceName) => $"{projectdir}{sep}Shared{sep}Interfaces{sep}I{serviceName}Service.cs";
    public static string ServicePath(string serviceName) => $"{projectdir}{sep}Services{sep}AllServices{sep}{serviceName}Service.cs";
    public static string RazorPath(string serviceName) => $"{projectdir}{sep}Client{sep}Pages{sep}{serviceName}";
    public static bool CanOverrideFile = false;
    public static bool IsDebug = false;

    public static string GetApplicationRoot(string path)
    {
        var exePath = Path.GetDirectoryName(path);
        Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
        var appRoot = appPathMatcher.Match(exePath!).Value;
        return appRoot;
    }
}
