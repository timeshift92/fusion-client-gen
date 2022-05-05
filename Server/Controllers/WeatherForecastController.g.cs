
        using Microsoft.AspNetCore.Mvc;
        using Stl.Fusion.Server;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Server.Controllers;

        [Route("api/[controller]/[action]")]
        [ApiController, JsonifyErrors]
        public class WeatherForecastController : ControllerBase, IWeatherForecastService
        {
            private readonly IWeatherForecastService _weatherforecast;

            public WeatherForecastController(IWeatherForecastService weatherforecast) => _weatherforecast = weatherforecast;

            
        [HttpGet,Publish]
        public System.Threading.Tasks.Task<Uztelecom.Template.Shared.WeatherForecast[]> GetForecast(DateTime startDate, CancellationToken cancellationToken ) => _weatherforecast.GetForecast(startDate, cancellationToken);
         

        }