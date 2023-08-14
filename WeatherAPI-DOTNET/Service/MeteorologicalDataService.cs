using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service.Interfaces;

namespace WeatherAPI_DOTNET.Service;

public class MeteorologicalDataService : IMeteorologicalDataService
{
    private MeteorologicalDataContext _context;
    private IMapper _mapper;
    public MeteorologicalDataService(MeteorologicalDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public  MeteorologicalDataEntity findMeteorologicalDataByID(int id)
    {
        MeteorologicalDataEntity metData = _context.MeteorologicalData.FirstOrDefault(metData => metData.Id == id);
        return metData;

    }
    
    public MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(DateTime date, List<MeteorologicalDataEntity> metDataList)
    {
        MeteorologicalDataEntity metData = metDataList.FirstOrDefault(
            metData =>
            (metData.WeatherDate.Day == date.Day) &&
            (metData.WeatherDate.Month == date.Month) &&
            (metData.WeatherDate.Year == date.Year)
            );
        return metData;
    }

    public List<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName)
    {
        List<MeteorologicalDataEntity> metDataList = _context.MeteorologicalData.Where(metDataList => metDataList.City == cityName).ToList();
        return metDataList;
    }

}
