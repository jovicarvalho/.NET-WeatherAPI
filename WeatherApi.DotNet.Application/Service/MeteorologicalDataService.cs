using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using WeatherApi.DotNet.Application.ExceptionHandling;
using WeatherApi.DotNet.Domain.Dtos;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Data.Repository;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service.Interfaces;

namespace WeatherAPI_DOTNET.Service;

public class MeteorologicalDataService : IMeteorologicalDataService
{
    private readonly IMeteorologicalDataRepository _repository;
    private readonly IMapper _mapper;
    public MeteorologicalDataService(IMeteorologicalDataRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedQueryWeather> FindAllMeteorologicalDataPaginated(int skip)
    {
        int pageSize = 10;
        var paginatedList = await _repository.GetPaginatedDataOfAllWeathers(skip * pageSize);
        return paginatedList?.weathers is null || !paginatedList.weathers.Any() ?
            throw new ServiceLayerNullPointerException("There is no weather data found!")
            : paginatedList;

    }

    public async Task<MeteorologicalDataEntity> FindMeteorologicalDataByID(Guid id)
    {
        var metData = await _repository.FindByID(id);
        return metData is null ?
            throw new ServiceLayerNullPointerException("Weather data not found with this ID!") 
            : metData;
    }


    public async Task<MeteorologicalDataEntity> FindMeteoroloficalDataBySpecificDate(string cityName, DateTime date)
    {
        var metData = await _repository.FindBySpecificDateAndCity(cityName, date);
        return metData is null ? 
            throw new ServiceLayerNullPointerException("There is no weathers found in this specific date in this City!")
            : metData;
    }

    public async Task<IEnumerable<MeteorologicalDataEntity>> FindWeekInCity(string cityName)
    {
        var weekInCity = await _repository.FindWeekInCity(cityName);
        return !weekInCity.Any() ?
            throw new ServiceLayerNullPointerException("Weather data not found with this ID!")
            : weekInCity;
    } 
    public async Task<MeteorologicalDataEntity> FindActualDay(string cityName)
    {
        var actualDay = DateTime.Now.Date;
        var metDataWithActualDay = await FindMeteoroloficalDataBySpecificDate(cityName, actualDay);
        return metDataWithActualDay;
    }

    public async Task<PaginatedQueryWeather> FindMeteorologicalDataByCityName(string cityName, int skip)
    {
        int pageSize = 10;
        var paginatedList = await _repository.GetPaginatedDataByCity(cityName, skip * pageSize);
        return paginatedList?.weathers is null || !paginatedList.weathers.Any()  ? 
            throw new ServiceLayerNullPointerException("There is no meteorological data with this city!")
            : paginatedList;

    }

    public async Task<MeteorologicalDataEntity> CreateMeteorologicalData(MeteorologicalDataDto metDataDto)
    {
        MeteorologicalDataEntity metData = _mapper.Map<MeteorologicalDataEntity>(metDataDto);
        await _repository.Add(metData);
        return metData;
    }

    public async Task<MeteorologicalDataEntity> EditMeteorologicalData(Guid id, MeteorologicalDataDto metDataDto)
    {
        var metData = await FindMeteorologicalDataByID(id);
        _mapper.Map(metDataDto, metData);
        await _repository.EditMeteorologicalData();
        return metData;
    }


    public async Task<MeteorologicalDataEntity> EditOnlyOneField(Guid id, JsonPatchDocument<MeteorologicalDataDto> patch)
    {
        var metDatainRepository = await FindMeteorologicalDataByID(id);
        var uptadeDto = _mapper.Map<MeteorologicalDataDto>(metDatainRepository);
        patch.ApplyTo(uptadeDto);
        var metDataAlreadyEdited = _mapper.Map(uptadeDto, metDatainRepository);
        await _repository.EditMeteorologicalData();
        return metDataAlreadyEdited;
    }

    public async Task<MeteorologicalDataEntity> DeleteMeteorologicalData(Guid id)
    {
#pragma warning disable CS8603

        var weather = await FindMeteorologicalDataByID(id);

        var deletedWeather = await _repository.DeleteById(weather);
       
       return deletedWeather;

#pragma warning restore CS8603 
    }

}
