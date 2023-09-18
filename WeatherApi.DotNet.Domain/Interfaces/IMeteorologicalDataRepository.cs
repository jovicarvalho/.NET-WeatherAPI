using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository.Interfaces
{
    public interface IMeteorologicalDataRepository
    {
        void Add(MeteorologicalDataEntity metData);
        IEnumerable<MeteorologicalDataEntity> GetAll(int skip);
        IEnumerable<MeteorologicalDataEntity> FindByCity(string city);
        MeteorologicalDataEntity FindByID(Guid id);
        MeteorologicalDataEntity FindBySpecificDateAndCity(string cityName, DateTime date);
        void EditMeteorologicalData();
        MeteorologicalDataEntity DeleteById(Guid id);
    }
}
    