
        using RestEase;
        using Uztelecom.Template.Shared;

        namespace Uztelecom.Template.Client.Services;

        [BasePath("WeatherForecast")]
        public interface IWeatherForecastClientDef
        {
        
        [Get("GetForecast")]
        public System.Threading.Tasks.Task<Uztelecom.Template.Shared.WeatherForecast[]> GetForecast(DateTime startDate, CancellationToken cancellationToken = default );
         

        }
        