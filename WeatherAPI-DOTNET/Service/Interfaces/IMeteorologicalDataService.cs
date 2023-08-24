using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Service.Interfaces
{
    public interface IMeteorologicalDataService
    {
        MeteorologicalDataEntity FindMeteorologicalDataByID(int id);
        List<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName);
        MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(DateTime dateOnly, string cityName);
        MeteorologicalDataEntity FindActualDay(string cityName);
        MeteorologicalDataEntity CreateMeteorologicalData(CreateMetDataDto metDataDto);
        IEnumerable<MeteorologicalDataEntity> FindAllMeteorologicalData(int skip);
        MeteorologicalDataEntity EditMeteorologicalData(int id, UpdateMetDataDto metDataDto);
        MeteorologicalDataEntity EditOnlyOneField(int id, JsonPatchDocument<UpdateMetDataDto> patch);
        MeteorologicalDataEntity DeleteMeteorologicalData(int id);

    }
}
