using FusionGenerator.Extensions;
using FusionGenerator.Extensions.Pluralization;
using FusionGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FusionGenerator.Extensions;

public static class RoslynExtensions
{
    public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
    {
        return type.GetBaseTypesAndThis().SelectMany(n => n.GetMembers());
    }

    public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode)
    {
        return syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
    }

    public static string GetClassName(this ClassDeclarationSyntax proxy) => proxy.Identifier.Text;

    public static string GetClassModifier(this ClassDeclarationSyntax proxy) => proxy.Modifiers.ToFullString().Trim();

    public static bool HaveAttribute(this ClassDeclarationSyntax classSyntax, string attributeName)
    {
        return classSyntax.AttributeLists.Count > 0 &&
               classSyntax.AttributeLists.SelectMany(al => al.Attributes
                                         .Where(a => (a.Name as IdentifierNameSyntax)!.Identifier.Text == attributeName))
                                         .Any();
    }
    public static bool HaveAttribute(this PropertyDeclarationSyntax classSyntax, string attributeName)
    {
        return classSyntax.AttributeLists.Count > 0 &&
               classSyntax.AttributeLists.SelectMany(al => al.Attributes
                                         .Where(a => (a.Name as IdentifierNameSyntax)!.Identifier.Text == attributeName))
                                         .Any();
    }


    public static string GetNamespace(this CompilationUnitSyntax root)
    {
        return root.ChildNodes()
                   .OfType<NamespaceDeclarationSyntax>()
                   .FirstOrDefault()
                   .Name
                   .ToString();
    }

    public static List<string> GetUsings(this CompilationUnitSyntax root)
    {
        return root.ChildNodes()
                   .OfType<UsingDirectiveSyntax>()
                   .Select(n => n.Name.ToString())
                   .ToList();
    }


    public static ImmutableArray<ClassMeta> ParseMeta(ImmutableArray<RecordDeclarationSyntax> classes, GlobalOptions opt) => classes.Select(cls =>
             new ClassMeta()
             {
                 Name = cls.Identifier.Text,
                 ServiceName = cls.Identifier.Text.Replace("Entity", ""),
                 PluralName = Pluralizer.Pluralize(cls.Identifier.Text.Replace("Entity", "")),
                 LowerPluralName = Pluralizer.Pluralize(cls.Identifier.Text.Replace("Entity", "")).ToLower(),
                 LowerName = cls.Identifier.Text.Replace("Entity", "").ToLower(),
                 Namespace = opt.GetNamespace(cls),
                 Rootnamespace = opt.RootNamespace!,
                 Primary = cls.Members.Select(member =>
                     new PropertyMeta()
                     {
                         Name = (member as PropertyDeclarationSyntax)!.Identifier.Text,
                         Type = (member as PropertyDeclarationSyntax)!.Type.ToString(),
                         IsPrimary = (member as PropertyDeclarationSyntax)!.HaveAttribute("Key")
                     }
                 ).ToImmutableArray().First(x => x.IsPrimary),
                 TypeParameters = cls.Members.Select(member =>

                     new PropertyMeta()
                     {
                         Name = (member as PropertyDeclarationSyntax)!.Identifier.Text,
                         Type = (member as PropertyDeclarationSyntax)!.Type.ToString(),
                         IsPrimary = (member as PropertyDeclarationSyntax)!.HaveAttribute("Key")
                     }
                 ).ToImmutableArray()
             }
         ).ToImmutableArray();
}