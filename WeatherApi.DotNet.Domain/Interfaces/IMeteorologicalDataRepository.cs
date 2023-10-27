using System.Runtime.CompilerServices;
using WeatherApi.DotNet.Domain.Dtos;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository.Interfaces
{
    public interface IMeteorologicalDataRepository
    {
        Task Add(MeteorologicalDataEntity metData);
        Task<PaginatedQueryWeather?> GetPaginatedDataOfAllWeathers(int skip);
        Task<PaginatedQueryWeather?> GetPaginatedDataByCity(string cityName, int skip);
        Task<IEnumerable<MeteorologicalDataEntity>> FindWeekInCity(string cityName);
        Task<MeteorologicalDataEntity?> FindByID(Guid id);
        Task<MeteorologicalDataEntity?> FindBySpecificDateAndCity(string cityName, DateTime date);
        Task EditMeteorologicalData();
        Task<MeteorologicalDataEntity?> DeleteById(MeteorologicalDataEntity weather);
    }
}
    