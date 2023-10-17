using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherApi.DotNet.Domain.Entity;

namespace WeatherAPI_DOTNET.Service.Interfaces
{
    public interface IMeteorologicalDataService
    {
        MeteorologicalDataEntity FindMeteorologicalDataByID(Guid id);
        PaginatedQueryWeather FindMeteorologicalDataByCityName(string cityName, int page);
        MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(string cityName, DateTime dateOnly);
        IEnumerable<MeteorologicalDataEntity> findWeekInCity(string cityName);
        MeteorologicalDataEntity FindActualDay(string cityName);
        MeteorologicalDataEntity CreateMeteorologicalData(CreateMetDataDto metDataDto);
        PaginatedQueryWeather FindAllMeteorologicalDataPaginated(int page);
        MeteorologicalDataEntity EditMeteorologicalData(Guid id, UpdateMetDataDto metDataDto);
        MeteorologicalDataEntity EditOnlyOneField(Guid id, JsonPatchDocument<UpdateMetDataDto> patch);
        MeteorologicalDataEntity DeleteMeteorologicalData(Guid id);

    }
}
