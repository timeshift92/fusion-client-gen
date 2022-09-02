
namespace FusionGenerator;

public interface IGenerator
{
    ServiceMetadata Type { get; }

    GeneratorMetadata Init();
}
