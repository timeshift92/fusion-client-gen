namespace Generator.Helpers;

public class MethodType
{

    public string? MethodName { get; set; }
    public string? MethodDeclaration { get; set; }
    public string? MethodParameters { get; set; } = "";
    public string? HttpMethodType { get; set; }

    public string GenerateControllerBody(string serviceName)
    {
        var attributes = $"[Http{HttpMethodType}]";
        if (HttpMethods.GET == HttpMethodType)
        {
            attributes = $"[Http{HttpMethodType},Publish]";
        }

        return @$"
        {attributes}
        public {MethodDeclaration} => _{serviceName.ToLower()}.{MethodName}({MethodParameters});
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