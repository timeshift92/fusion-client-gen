namespace FusionGenerator
{
    public class ServiceMetadata
    {
        public string? Name { get; set; }
        public List<MethodMeta> MethodSymbols { get; set; } = new List<MethodMeta>();
    }

    public class MethodMeta
    {
        public string ReturnType { get; set; } = null!;
        public List<string> Arguments { get; set; } = new List<string>();
        public List<string> Attributes { get; set; } = new List<string>();
        public string ParentType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FullString => $"{ReturnType} {Name}({string.Join(",",Arguments)})";
        public bool IsSessionCommand { get; set; }

        public string Generate(string httpMethodType, bool isServer = false)
        {

            var bodyType = isServer ? "[FromBody]" : "[Body]";
            var withBody = httpMethodType != HttpMethods.GET ? bodyType : "";
            var splitted = FullString.Split('(');
            splitted[1] = $"{withBody} {splitted[1]}";
            splitted[1] = splitted[1].Replace(";", "");
            if (httpMethodType == HttpMethods.GET && splitted[1].IndexOf(',') != -1 && splitted[1].Contains("Options"))
            {
                splitted[1] = isServer ? $"[FromQuery] {splitted[1]}" : $"[Query] {splitted[1]}";
            }
            return $"{splitted[0]}({splitted[1]}";
        }
    }
}
