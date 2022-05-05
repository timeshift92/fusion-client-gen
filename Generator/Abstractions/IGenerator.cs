using Uztelecom.Template.Generator.Helpers;

namespace Generator.Abstractions;

public interface IGenerator
{
    ServiceMetadata Type { get; }

    GeneratorMetadata Init();
}
