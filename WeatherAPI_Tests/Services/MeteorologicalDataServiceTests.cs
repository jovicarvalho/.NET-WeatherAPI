using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service;
using WeatherAPI_DOTNET.Service.Interfaces;
using Xunit;

namespace WeatherAPI_Tests.Services;

public class MeteorologicalDataServiceTests
{


    private Mock<IMeteorologicalDataRepository> _repository;
    private Mock<IMapper> _mapper;
    private MeteorologicalDataService _service;

    public MeteorologicalDataServiceTests()
    {
        _repository = new Mock<IMeteorologicalDataRepository>();
        _mapper = new Mock<IMapper>();
        _service = new MeteorologicalDataService(_repository.Object, _mapper.Object);

    }

    private CreateMetDataDto createCorrectDTO = new CreateMetDataDto
    {
        City = "Roma",
        WeatherDate = new DateTime(2023, 2, 3),
        MorningWeather = "Sunny",
        NightWeather = "Sunny",
        MaxTemperature = 70,
        MinTemperature = 0,
        humidity = 20,
        WindSpeed = 20,
        Precipitation = 20
    };


    private CreateMetDataDto createWrongDTONullCity = new CreateMetDataDto
    {
        City = null,
        WeatherDate = new DateTime(2023, 2, 3),
        MorningWeather = "Sunny",
        NightWeather = "Sunny",
        MaxTemperature = 70,
        MinTemperature = 0,
        humidity = 20,
        WindSpeed = 20,
        Precipitation = 20
    };


    [Fact]
    public void Post_SendingValidEntity()
    {
        //  Arrange         
        _mapper.Setup(m => m.Map<CreateMetDataDto, MeteorologicalDataEntity>(It.IsAny<CreateMetDataDto>()))
              .Returns((CreateMetDataDto source) => new MeteorologicalDataEntity
              {
                  City = source.City,
                  WeatherDate = source.WeatherDate,
                  MorningWeather = source.MorningWeather,
                  NightWeather = source.NightWeather,
                  MaxTemperature = source.MaxTemperature,
                  MinTemperature = source.MinTemperature,
                  humidity = source.humidity,
                  WindSpeed = source.WindSpeed,
                  Precipitation = source.Precipitation
              });
        _mapper.Setup(m => m.Map<MeteorologicalDataEntity>(createCorrectDTO)).Returns((CreateMetDataDto source) => new MeteorologicalDataEntity
        {
            City = source.City,
            WeatherDate = source.WeatherDate,
            MorningWeather = source.MorningWeather,
            NightWeather = source.NightWeather,
            MaxTemperature = source.MaxTemperature,
            MinTemperature = source.MinTemperature,
            humidity = source.humidity,
            WindSpeed = source.WindSpeed,
            Precipitation = source.Precipitation
        });

        //Act
        var result = _service.CreateMeteorologicalData(createCorrectDTO);


        //Assert
        Assert.NotNull(result);
        Assert.Equal(result.City, createCorrectDTO.City);
        Assert.Equal(result.WeatherDate, createCorrectDTO.WeatherDate);
        Assert.Equal(result.NightWeather, createCorrectDTO.NightWeather);
        Assert.Equal(result.MorningWeather, createCorrectDTO.MorningWeather);
        Assert.Equal(result.MinTemperature, createCorrectDTO.MinTemperature);
        Assert.Equal(result.MaxTemperature, createCorrectDTO.MaxTemperature);
        Assert.Equal(result.WindSpeed, createCorrectDTO.WindSpeed);
        Assert.Equal(result.humidity, createCorrectDTO.humidity);
        Assert.Equal(result.Precipitation, createCorrectDTO.Precipitation);

        _repository.Verify(r=>r.Add(It.IsAny<MeteorologicalDataEntity>()), Times.Once);
    }

    [Fact]
    public void GetByID_ValidID()
    {
        //Arrange

        MeteorologicalDataEntity metDataWithID = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r=>r.FindByID(metDataWithID.Id)).Returns(metDataWithID);

        //Act
        var result = _service.FindMeteorologicalDataByID(metDataWithID.Id);

        //Assert
        Assert.Equal(metDataWithID, result);
    }

}
