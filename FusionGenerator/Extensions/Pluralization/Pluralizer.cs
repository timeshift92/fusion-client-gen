using System;
using System.Collections.Generic;
using System.Text;

namespace FusionGenerator.Extensions.Pluralization
{
    public static class Pluralizer
    {
        public static string Pluralize(string name)
        {
            return name.Pluralize() ?? name;
        }
        public static string Singularize(string name)
        {
            return name.Singularize() ?? name;
        }
    }
}
