using WeatherApi.DotNet.Domain.Entity;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository.Interfaces
{
    public interface IMeteorologicalDataRepository
    {
        void Add(MeteorologicalDataEntity metData);
        public Task<PaginatedQueryWeather> GetPaginatedDataOfAllWeathers(int skip);
        public Task<PaginatedQueryWeather> GetPaginatedDataByCity(string cityName, int page);
        IEnumerable<MeteorologicalDataEntity> FindWeekInCity(string cityName);
        MeteorologicalDataEntity FindByID(Guid id);
        MeteorologicalDataEntity FindBySpecificDateAndCity(string cityName, DateTime date);
        void EditMeteorologicalData();
        MeteorologicalDataEntity DeleteById(Guid id);
    }
}
    