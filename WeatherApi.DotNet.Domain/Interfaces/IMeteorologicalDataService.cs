using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Data.Dtos;

namespace WeatherAPI_DOTNET.Service.Interfaces
{
    public interface IMeteorologicalDataService
    {
        MeteorologicalDataEntity FindMeteorologicalDataByID(Guid id);
        IEnumerable<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName);
        MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(string cityName, DateTime dateOnly);
        MeteorologicalDataEntity FindActualDay(string cityName);
        MeteorologicalDataEntity CreateMeteorologicalData(CreateMetDataDto metDataDto);
        IEnumerable<MeteorologicalDataEntity> FindAllMeteorologicalData(int skip);
        MeteorologicalDataEntity EditMeteorologicalData(Guid id, UpdateMetDataDto metDataDto);
        MeteorologicalDataEntity EditOnlyOneField(Guid id, JsonPatchDocument<UpdateMetDataDto> patch);
        MeteorologicalDataEntity DeleteMeteorologicalData(Guid id);

    }
}
