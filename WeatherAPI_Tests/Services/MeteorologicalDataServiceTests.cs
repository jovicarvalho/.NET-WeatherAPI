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
    
    [Fact(DisplayName ="Create With Valid Entity - Sucess")]
    public void PostSucess_SendingValidEntity()
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
    }
    [Fact(DisplayName = "Create - Verify if calls Add method in Repository, correctly")]
    public void Post_VerifysCommunicationWithRepository()
    {
        var result = _service.CreateMeteorologicalData(createCorrectDTO);
        _repository.Verify(r => r.Add(It.IsAny<MeteorologicalDataEntity>()), Times.Once);
    }

    [Fact(DisplayName ="Get by Id With Valid Id - Sucess")]
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

    [Fact(DisplayName ="Get by Id With Invalid Id - Failed")]
    public void GetByIDFailed_InvalidID()
    {
        //Arrange

        MeteorologicalDataEntity metDataWithID = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.FindByID(metDataWithID.Id)).Returns(metDataWithID);

        //Act
        var result = _service.FindMeteorologicalDataByID(1);

        //Assert
        Assert.Null(result);
    }

    [Fact(DisplayName ="Get by City With Valid City - Sucess")]
    public void GetByCityName_ValidCity(){
        //Arrange
        MeteorologicalDataEntity metDataToSearchByCity = new Fixture().Create<MeteorologicalDataEntity>();
        MeteorologicalDataEntity metDataToSearchByCity2 = new Fixture().Create<MeteorologicalDataEntity>();
        IEnumerable<MeteorologicalDataEntity> metDataList = new []{ metDataToSearchByCity, metDataToSearchByCity2 };
        _repository.Setup(r => r.FindByCity(metDataToSearchByCity.City)).Returns(metDataList);
        IEnumerable<MeteorologicalDataEntity> metDataListOrdenated = metDataList
            .OrderByDescending(metData => metData.WeatherDate)
            .Take(7);
        //Act
        var response = _service.FindMeteorologicalDataByCityName(metDataToSearchByCity.City);
        //Assert
        Assert.Equal(metDataListOrdenated, response);
    }

    [Fact(DisplayName ="Get by City With Invalid City - Failed")]
    public void GetByCityNameFailed_InValidCity()
    {
        //Arrange
        MeteorologicalDataEntity metDataToSearchByCity = new Fixture().Create<MeteorologicalDataEntity>();
        MeteorologicalDataEntity metDataToSearchByCity2 = new Fixture().Create<MeteorologicalDataEntity>();
        IEnumerable<MeteorologicalDataEntity> metDataList = new[] { metDataToSearchByCity, metDataToSearchByCity2 };
        _repository.Setup(r => r.FindByCity(metDataToSearchByCity.City)).Returns(metDataList);
        IEnumerable<MeteorologicalDataEntity> metDataListOrdenated = metDataList
            .OrderByDescending(metData => metData.WeatherDate)
            .Take(7);
        var emptyList = new List<MeteorologicalDataEntity>();
        //Act
        var response = _service.FindMeteorologicalDataByCityName("testCity");
        //Assert
        Assert.Equal(emptyList,response);
     }

    [Fact(DisplayName ="Get by Date(Actual Day) and City - Sucess")]
    public void GetByActualDay()
    {
        //Arrange
        var actualDay = DateTime.Now.Date;
        var metData = new MeteorologicalDataEntity
        {
            Id= 1,
            City = "Roma",
            WeatherDate = actualDay,
            MorningWeather = "Sunny",
            NightWeather = "Sunny",
            MaxTemperature = 70,
            MinTemperature = 0,
            humidity = 20,
            WindSpeed = 20,
            Precipitation = 20
        };

        _repository.Setup(r => r.FindBySpecificDateAndCity(metData.City, metData.WeatherDate)).Returns(metData);
        //Act
        var response = _service.FindActualDay(metData.City);
        //Assert
        Assert.Equal(metData, response);
        _repository.Verify(r => r.FindBySpecificDateAndCity(metData.City, actualDay), Times.Once);
    }

    [Fact(DisplayName ="Get by Date and City - Sucess")]
    public void GetBySpecificDate_ValidDate()
    {
        //Arrange
        var metData = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.FindBySpecificDateAndCity(metData.City, metData.WeatherDate)).Returns(metData);
        //Act
        var response = _service.FindMeteoroloficalDataBySpecificDate(metData.City, metData.WeatherDate);
        //Assert
        Assert.Equal(metData, response);
        _repository.Verify(r => r.FindBySpecificDateAndCity(metData.City, metData.WeatherDate), Times.Once);
    }

    [Fact(DisplayName ="Get All Meteorological datas - Sucess")]
    public void GetAllMeteorologicalDataSucessCall()
    {
        //Arrange
        IEnumerable<MeteorologicalDataEntity> metDataList = new Fixture().Create<IEnumerable<MeteorologicalDataEntity>>();
        _repository.Setup(r=>r.GetAll()).Returns(metDataList);
        var skip = 0;
        //Act
        var response = _service.FindAllMeteorologicalData(skip);
        //Assert
        Assert.Equal(metDataList, response);
    }

    [Fact(DisplayName ="Delete by Id with Valid - Sucess ")]
    public void Delete_ValidId()
    {
        // Arrange
        MeteorologicalDataEntity metData = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r=>r.DeleteById(metData.Id)).Returns(metData);
        //Act
        var response = _service.DeleteMeteorologicalData(metData.Id);
        //Assert
        Assert.Equal(metData,response); 
        _repository.Verify(r=>r.DeleteById(metData.Id), Times.Once);
    }
    [Fact(DisplayName = "Delete by ID - Verify if calls Delete method in Repository, correctly")]
    public void Delete()
    {
        int id = 0; 
        var response = _service.DeleteMeteorologicalData(id);
        //Assert
        _repository.Verify(r => r.DeleteById(id), Times.Once);
    }
    [Fact(DisplayName ="Delete by Id with Invalid Id - Failed")]
    public void DeleteFailed_InvalidId()
    {
        // Arrange
        MeteorologicalDataEntity metData = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.DeleteById(metData.Id)).Returns(metData);
        //Act
        var response = _service.DeleteMeteorologicalData(10);
        //Assert
        Assert.NotEqual(metData, response);
        
    }
}
