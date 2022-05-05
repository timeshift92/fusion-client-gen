
        using Microsoft.AspNetCore.Mvc;
        using Stl.Fusion.Server;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Server.Controllers;

        [Route("api/[controller]/[action]")]
        [ApiController, JsonifyErrors]
        public class CounterController : ControllerBase, ICounterService
        {
            private readonly ICounterService _counter;

            public CounterController(ICounterService counter) => _counter = counter;

            
        [HttpGet,Publish]
        public System.Threading.Tasks.Task<int> Get(CancellationToken cancellationToken ) => _counter.Get(cancellationToken);
         

        [HttpGet,Publish]
        public System.Threading.Tasks.Task Increment(CancellationToken cancellationToken ) => _counter.Increment(cancellationToken);
         

        }