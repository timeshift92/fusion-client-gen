using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Linq;
using System;

namespace FusionGenerator.Extensions;

public sealed record GlobalOptions // this must be a record or implement IEquatable<T>
{
    public string? RootNamespace;
    public string? ProjectFullPath;

    public bool IsValid { get; }
    public string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
                potentialNamespaceParent is not NamespaceDeclarationSyntax
                && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
    public static string GetAssemblyDir() => Path.GetDirectoryName(GetAssemblyPath());
    public static string GetAssemblyPath() => Assembly.GetExecutingAssembly().Location;
    public string? CurrentDirPath = GetAssemblyDir();

    public string projectdir = "";
    public static char sep = Path.DirectorySeparatorChar;
    public string CommandPath(string serviceName) => $"{projectdir}{sep}Shared{sep}Commands{sep}{serviceName}Commands.cs";
    public string DbContextPath => $"{projectdir}{sep}Services{sep}AppDbContext.g.cs";
    public string FusionServerExtension => $"{projectdir}{sep}Server{sep}FusionServerExtension.cs";
    public string InterfacePath(string serviceName) => $"{projectdir}{sep}Shared{sep}Interfaces{sep}I{serviceName}Service.cs";
    public string ServicePath(string serviceName) => $"{projectdir}{sep}Services{sep}AllServices{sep}{serviceName}Service.cs";
    public string RazorPath(string serviceName) => $"{projectdir}{sep}Client{sep}Pages{sep}{serviceName}";
    public bool CanOverrideFile = false;
    public bool IsDebug = false;

    public string GetSolutionPath()
    {
        while (CurrentDirPath != null)
        {
            var fileInCurrentDir = Directory.GetFiles(CurrentDirPath).Select(f => f.Split(Path.DirectorySeparatorChar).Last()).ToArray();
            var solutionFileName = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase));
            if (solutionFileName != null)
                return Path.Combine(CurrentDirPath, solutionFileName);

            CurrentDirPath = Directory.GetParent(CurrentDirPath)!.FullName;
        }

        throw new FileNotFoundException("Cannot find solution file path");
    }

    public string GetSolutionDir() => Directory.GetParent(GetSolutionPath()).FullName;

    public string GetSolutionFileName() => GetSolutionPath().Split(Path.DirectorySeparatorChar).Last();
    private GlobalOptions(AnalyzerConfigOptions options)
    {
        IsValid = true;
        options.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFullPath);
        options.TryGetValue("build_property.RootNamespace", out RootNamespace);
        options.TryGetValue("build_property.projectdir", out var currentDirPath);
        options.TryGetValue("build_property.SourceGenerator_Override", out var ovveridable);
        options.TryGetValue("build_property.SourceGenerator_EnableDebug", out var isDebug);

        IsDebug = !string.IsNullOrEmpty(isDebug);
        CanOverrideFile = !string.IsNullOrEmpty(ovveridable);


        ProjectFullPath = projectFullPath;
        CurrentDirPath = currentDirPath!;
        projectdir = Directory.GetParent(CurrentDirPath)?.Parent.FullName!;
        RootNamespace = GetSolutionFileName().Replace(".sln", "");

    }

    public static GlobalOptions Select(AnalyzerConfigOptionsProvider provider, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return new(provider.GlobalOptions);
    }

}



