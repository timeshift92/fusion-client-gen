using System.Collections.Generic;

namespace FusionGenerator.Abstractions;

public class GeneratorMetadata
{
    public List<string> Templates;

    public GeneratorMetadata(List<string> templates, string controllerName)
    {
        Templates = templates;
        ControllerName = controllerName;
    }

    public string ControllerName;
}
