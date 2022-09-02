using FusionGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace FusionGenerator.Helpers
{
    public class ServiceMetadata
    {
        public string? Name { get; set; }
        public List<MethodMeta> MethodSymbols { get; set; } = new List<MethodMeta>();
    }

    public class MethodMeta
    {
        public string Generate(string httpMethodType, bool isServer = false)
        {

            var bodyType = isServer ? "[FromBody]" : "[Body]";
            var withBody = httpMethodType != HttpMethods.GET ? bodyType : "";
            var splitted = FullString.Split('(');
            splitted[1] = $"{withBody} {splitted[1]}";
            splitted[1] = splitted[1].Replace(";", "");
             if(httpMethodType == HttpMethods.GET && splitted[1].IndexOf(',') != -1 && splitted[1].Contains("Options") )
            {
                splitted[1] = isServer ?  $"[FromQuery] {splitted[1]}": $"[Query] {splitted[1]}";
            }
            return $"{splitted[0]}({splitted[1]}";
        }
        public MethodMeta(MethodDeclarationSyntax syntax)
        {
            Name = syntax.Identifier.Text;
            if (syntax.AttributeLists.Count > 0)
            {
                FullString = syntax.ToString().Replace(syntax.AttributeLists[0].ToString(), "").Trim();
            }
            else
            {
                FullString = syntax.ToString().Trim();
            }

            ReturnType = syntax.ReturnType.ToFullString();
            foreach (var item in syntax.ParameterList.Parameters)
            {
                Parameters.Add((item as ParameterSyntax).ToFullString());

            }

            var symbol = FusionServicesGenerator.Compilation
                .GetSemanticModel(syntax.ParameterList.Parameters[0].SyntaxTree)
                .GetDeclaredSymbol(syntax.ParameterList.Parameters[0]);
            if (symbol!.ContainingAssembly.GetTypeByMetadataName(symbol.OriginalDefinition.ToDisplayString()) is not null)
            {
                IsSessionCommand = symbol.ContainingAssembly
                .GetTypeByMetadataName(symbol.OriginalDefinition.ToDisplayString())!
                .AllInterfaces
                .Any(x => x.Name.Contains("ISession"));
            }

        }
        public string Name { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
        public string ReturnType { get; set; }
        public string FullString { get; set; }
        public bool IsSessionCommand { get; set; }
    }
}
