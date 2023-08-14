using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Service.Interfaces
{
    public interface IMeteorologicalDataService
    {
        MeteorologicalDataEntity findMeteorologicalDataByID(int id);
        List<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName);
        public MeteorologicalDataEntity FindMeteoroloficalDataSpecific(DateTime dateOnly, List<MeteorologicalDataEntity> metDataList);
    }
}
