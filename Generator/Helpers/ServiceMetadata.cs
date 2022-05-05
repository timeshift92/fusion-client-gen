using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uztelecom.Template.Generator.Helpers
{
    public class ServiceMetadata
    {
        public string Name { get; set; }
        public List<IMethodSymbol> methodSymbols { get; set; }
    }
}
