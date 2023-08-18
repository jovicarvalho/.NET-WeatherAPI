using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository.Interfaces
{
    public interface IMeteorologicalDataRepository
    {
        void Add(MeteorologicalDataEntity metData);
        MeteorologicalDataEntity FindByID(int id);
    }
}
