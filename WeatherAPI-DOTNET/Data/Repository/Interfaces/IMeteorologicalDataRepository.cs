using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository.Interfaces
{
    public interface IMeteorologicalDataRepository
    {
        void Add(MeteorologicalDataEntity metData);
        IEnumerable<MeteorologicalDataEntity> GetAll();
        IEnumerable<MeteorologicalDataEntity> FindByCity(string city);
        MeteorologicalDataEntity FindByID(int id);
        MeteorologicalDataEntity FindBySpecificDateAndCity(string cityName, DateTime date);
        MeteorologicalDataEntity DeleteById(int id);
    }
}
    