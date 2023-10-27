using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherApi.DotNet.Domain.Dtos;

namespace WeatherAPI_DOTNET.Service.Interfaces
{
    public interface IMeteorologicalDataService
    {
        Task<MeteorologicalDataEntity> FindMeteorologicalDataByID(Guid id);
        Task<PaginatedQueryWeather> FindMeteorologicalDataByCityName(string cityName, int skip);
        Task<MeteorologicalDataEntity> FindMeteoroloficalDataBySpecificDate(string cityName, DateTime date);
        Task<IEnumerable<MeteorologicalDataEntity>> FindWeekInCity(string cityName);
        Task<MeteorologicalDataEntity> FindActualDay(string cityName);
        Task<MeteorologicalDataEntity> CreateMeteorologicalData(MeteorologicalDataDto metDataDto);
        Task<PaginatedQueryWeather> FindAllMeteorologicalDataPaginated(int skip);
        Task<MeteorologicalDataEntity> EditMeteorologicalData(Guid id, MeteorologicalDataDto metDataDto);
        Task<MeteorologicalDataEntity> EditOnlyOneField(Guid id, JsonPatchDocument<MeteorologicalDataDto> patch);
        Task<MeteorologicalDataEntity> DeleteMeteorologicalData(Guid id);

    }
}
