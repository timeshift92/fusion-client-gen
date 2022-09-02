
namespace FusionGenerator;


public class MethodType
{

    public string? MethodName { get; set; }
    public string? MethodDeclaration { get; set; }
    public string? MethodParameters { get; set; } = "";
    public string? HttpMethodType { get; set; }
    public bool IsSession { get; set; }

    public string GenerateControllerBody(string serviceName)
    {
        var attributes = $"[Http{HttpMethodType}]";
        if (HttpMethods.GET == HttpMethodType)
        {
            attributes = $"[Http{HttpMethodType},Publish]";
        }

        string[] methods = { HttpMethods.GET, HttpMethods.DELETE };

        if (!methods.Contains(HttpMethodType))
        {
            var defaultSession = IsSession ? ".UseDefaultSession(_sessionResolver)" : "";
            var parameters = MethodParameters!.Split(',');
            parameters[0] = !parameters[0].Contains("UseDefaultSession") ? $"{parameters[0].Trim()}{defaultSession}" : parameters[0];
            MethodParameters = string.Join(",", parameters);
        }


        return @$"
        {attributes}
        public {MethodDeclaration} {{
            return _{serviceName.ToLower()}.{MethodName}({MethodParameters});
            }} 
        ";

    }

    public string GenerateClientBody()
    {
        var attributes = $@"[{HttpMethodType}(""{MethodName}"")]";

        return @$"
        {attributes}
        public {MethodDeclaration};
        ";
    }


}