namespace Generator.Helpers;

public class HttpMethods
{
    public  const string POST = "Post";
    public const string GET = "Get";
    public const string PUT = "Put";
    public const string DELETE = "Delete";


    public static Func<string, string> DefineHttpMethodByMethodName = (string methodName) => methodName switch
{
    string s when s.ToLower().IndexOf("post") != -1 => POST,
    string s when s.ToLower().IndexOf("get") != -1 => GET,
    string s when s.ToLower().IndexOf("put") != -1 => PUT,
    string s when s.ToLower().IndexOf("delete") != -1 => DELETE,
    _ => GET
};


}