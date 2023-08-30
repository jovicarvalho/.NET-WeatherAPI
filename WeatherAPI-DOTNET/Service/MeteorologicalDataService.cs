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

    public IEnumerable<MeteorologicalDataEntity> FindAllMeteorologicalData(int skip)
    {
        return _repository.GetAll(skip * 10);
    }

    public MeteorologicalDataEntity FindMeteorologicalDataByID(Guid id)
    {
        var metData = _repository.FindByID(id);
        return metData;
    }


    public MeteorologicalDataEntity FindMeteoroloficalDataBySpecificDate(string cityName, DateTime date)
    {
        MeteorologicalDataEntity metData = _repository.FindBySpecificDateAndCity(cityName, date);
        return metData;
    }

    public MeteorologicalDataEntity FindActualDay(string cityname)
    {
        var actualDay = DateTime.Now.Date;
        MeteorologicalDataEntity metDataWithActualDay = FindMeteoroloficalDataBySpecificDate(cityname, actualDay);
        return metDataWithActualDay;
    }

    public IEnumerable<MeteorologicalDataEntity> FindMeteorologicalDataByCityName(string cityName)
    {
        IEnumerable<MeteorologicalDataEntity> metDataList = _repository.FindByCity(cityName).ToList();
        return metDataList;
    }

    public MeteorologicalDataEntity CreateMeteorologicalData(CreateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metData = _mapper.Map<MeteorologicalDataEntity>(metDataDto);
        _repository.Add(metData);
        return metData;
    }

    public MeteorologicalDataEntity EditMeteorologicalData(Guid id, UpdateMetDataDto metDataDto)
    {
        var metData = FindMeteorologicalDataByID(id);
        _mapper.Map(metDataDto, metData);
        _repository.EditMeteorologicalData();
        return metData;
    }


    public MeteorologicalDataEntity EditOnlyOneField(Guid id, JsonPatchDocument<UpdateMetDataDto> patch)
    {
        var metDatainRepository = FindMeteorologicalDataByID(id);
        var uptadeDto = _mapper.Map<UpdateMetDataDto>(metDatainRepository);
        patch.ApplyTo(uptadeDto);
        var metDataAlreadyEdited = _mapper.Map(uptadeDto, metDatainRepository);
        _repository.EditMeteorologicalData();
        return metDataAlreadyEdited;
    }

    public MeteorologicalDataEntity DeleteMeteorologicalData(Guid id)
    {
        var metData = _repository.DeleteById(id);
        return metData;
    }

}
