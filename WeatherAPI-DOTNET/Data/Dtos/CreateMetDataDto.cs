using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static WeatherAPI_DOTNET.Service.MeteorologicalDataService;

namespace WeatherAPI_DOTNET.Data.Dtos
{
    public class CreateMetDataDto
    {
        [Required(ErrorMessage = "Informar a cidade é obrigatório.")]
        [StringLength(35, ErrorMessage = "O nome da cidade não deve exceder 50 caracteres")]
        public string City { get; set; }

        [Required(ErrorMessage = "Campo de data é obrigatório.")]
        public DateTime WeatherDate { get; set; }

        [Required(ErrorMessage = "Informar o clima da manhã é obrigatório")]
        public string MorningWeather { get; set; }

        [Required(ErrorMessage = "Informar o clima da noite é obrigatório")]
        public string NightWeather { get; set; }

        [Required(ErrorMessage = "O usuário deve informar a temperatura máxima do dia.")]
        public int MaxTemperature { get; set; }

        [Required(ErrorMessage = "O usuário deve informar a temperatura mínima do dia")]
        public int MinTemperature { get; set; }

        [Required(ErrorMessage = "O campo de humidade é obrigatório")]
        [Range(0, 100, ErrorMessage = "A humidade deve ser entre 0 e 100%")]
        public int Humidity { get; set; }

        [Required(ErrorMessage = "O campo de precipitação é obrigatório")]
        [Range(0, 100, ErrorMessage = "A precipitação deve ser entre 0 e 100%")]
        public int Precipitation { get; set; }

        [Required(ErrorMessage = "O campo de humidade é obrigatório")]
        [Range(0, 200, ErrorMessage = "A velocidade do vento deve ser maior que 0 e menor que 200 km/h")]
        public int WindSpeed { get; set; }
    }
}
