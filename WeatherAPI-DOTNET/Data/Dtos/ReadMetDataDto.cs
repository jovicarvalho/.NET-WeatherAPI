using System.ComponentModel.DataAnnotations;

namespace WeatherAPI_DOTNET.Data.Dtos
{
    public class ReadMetDataDto
    {
        public string City { get; set; }
        public DateTime WeatherDate { get; set; }
        public string MorningWeather { get; set; }
        public string NightWeather { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int humidity { get; set; }
        public int Precipitation { get; set; }
        public int WindSpeed { get; set; }
    }
}
