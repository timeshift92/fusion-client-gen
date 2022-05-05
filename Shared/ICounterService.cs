using Stl.Fusion;

namespace Uztelecom.Template.Shared;

public interface ICounterService
{
    [ComputeMethod]
    Task<int> Get(CancellationToken cancellationToken = default);
    Task Increment(CancellationToken cancellationToken = default);
}
