using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Data.Repository;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service.Interfaces;

namespace WeatherAPI_DOTNET.Service;

public class MeteorologicalDataService : IMeteorologicalDataService
{
    private IMeteorologicalDataRepository _repository;
    private IMapper _mapper;
    public MeteorologicalDataService(IMeteorologicalDataRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper; 
    }

    //public IEnumerable<MeteorologicalDataEntity> FindAllMeteorologicalData(int skip)
    //{
    //    return _context.MeteorologicalData.Skip(skip).Take(10);
    //}

    public  MeteorologicalDataEntity FindMeteorologicalDataByID(int id)
    {
        return _repository.FindByID(id);
    }

    
    //public MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(DateTime date, string cityName)
    //{
    //    List<MeteorologicalDataEntity> metDataList = FindMeteorologicalDataByCityName(cityName);
    //    MeteorologicalDataEntity? metData = metDataList.FirstOrDefault(
    //        metData =>
    //        (metData.WeatherDate.Day == date.Day) &&
    //        (metData.WeatherDate.Month == date.Month) &&
    //        (metData.WeatherDate.Year == date.Year)
    //        );
    //    return metData;
    //}

    //public MeteorologicalDataEntity FindActualDay(string cityName)
    //{
    //    DateTime actualDay = DateTime.Now;
    //    MeteorologicalDataEntity metDataWithActualDay = FindMeteoroloficalDataBySpecificDate(actualDay, cityName);
    //    return metDataWithActualDay;
    //}

    //public List<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName)
    //{
    //    List<MeteorologicalDataEntity> metDataList = _context.MeteorologicalData.Where(metDataList => metDataList.City == cityName).ToList();
    //    return metDataList;
    //}

    public MeteorologicalDataEntity CreateMeteorologicalData(CreateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metData = _mapper.Map<MeteorologicalDataEntity>(metDataDto);
        _repository.Add(metData);
        return metData;
    }

    //public MeteorologicalDataEntity EditMeteorologicalData(int id,UpdateMetDataDto metDataDto)
    //{
    //    var metData = FindMeteorologicalDataByID(id);
    //    _mapper.Map(metDataDto, metData);
    //    _context.SaveChanges();
    //    return metData;
    //}

    //public MeteorologicalDataEntity EditOnlyOneField(int id, JsonPatchDocument<UpdateMetDataDto> patch)
    //{
    //    var metData = FindMeteorologicalDataByID(id);
    //    var metDataAtualizar = _mapper.Map<UpdateMetDataDto>(metData);
    //    patch.ApplyTo(metDataAtualizar);
    //    _mapper.Map(metDataAtualizar, metData);
    //    _context.SaveChanges();
    //    return metData;
    //}

    //public MeteorologicalDataEntity DeleteMeteorologicalData(int id)
    //{
    //    var metData = FindMeteorologicalDataByID(id);
    //    _context.Remove(metData);
    //    _context.SaveChanges(); 
    //    return metData;
    //}
}
