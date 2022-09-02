using FusionGenerator.Helpers;

namespace FusionGenerator.Abstractions;

public interface IGenerator
{
    ServiceMetadata Type { get; }

    GeneratorMetadata Init();
}
