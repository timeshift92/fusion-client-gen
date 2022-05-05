
        using RestEase;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Client.Services;

        [BasePath("Counter")]
        public interface ICounterClientDef
        {
        
        [Get("Get")]
        public System.Threading.Tasks.Task<int> Get(CancellationToken cancellationToken = default );
         

        [Get("Increment")]
        public System.Threading.Tasks.Task Increment(CancellationToken cancellationToken = default );
         

        }
        