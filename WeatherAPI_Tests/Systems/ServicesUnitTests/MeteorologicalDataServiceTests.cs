using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.DotNet.Application.ExceptionHandling;
using WeatherApi.DotNet.Domain.Dtos;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service;
using WeatherAPI_DOTNET.Service.Interfaces;
using Xunit;

namespace WeatherApi.DotNet.Tests.Systems.ServicesUnitTests;

public class MeteorologicalDataServiceTests
{


    private readonly Mock<IMeteorologicalDataRepository> _repository;
    private readonly Mock<IMapper> _mapper;
    private readonly MeteorologicalDataService _service;

    public MeteorologicalDataServiceTests()
    {
        _repository = new Mock<IMeteorologicalDataRepository>();
        _mapper = new Mock<IMapper>();
        _service = new MeteorologicalDataService(_repository.Object, _mapper.Object);

    }

    private readonly MeteorologicalDataDto createCorrectDTO = new()
    {
        City = "Roma",
        WeatherDate = new DateTime(2023, 2, 3),
        MorningWeather = "Sunny",
        NightWeather = "Sunny",
        MaxTemperature = 70,
        MinTemperature = 0,
        Humidity = 20,
        WindSpeed = 20,
        Precipitation = 20
    };

    [Fact(DisplayName = "Create With Valid Entity - Sucess")]
    public async Task PostSucess_SendingValidEntity()
    {
        //  Arrange         

        _mapper.Setup(m => m.Map<MeteorologicalDataEntity>(createCorrectDTO)).Returns((MeteorologicalDataDto source) => new MeteorologicalDataEntity
        {
            City = source.City,
            WeatherDate = source.WeatherDate,
            MorningWeather = source.MorningWeather,
            NightWeather = source.NightWeather,
            MaxTemperature = source.MaxTemperature,
            MinTemperature = source.MinTemperature,
            Humidity = source.Humidity,
            WindSpeed = source.WindSpeed,
            Precipitation = source.Precipitation
        });

        //Act
        var result = await _service.CreateMeteorologicalData(createCorrectDTO);


        //Assert
        Assert.NotNull(result);
        Assert.Equal(result.City, createCorrectDTO.City);
        Assert.Equal(result.WeatherDate, createCorrectDTO.WeatherDate);
        Assert.Equal(result.NightWeather, createCorrectDTO.NightWeather);
        Assert.Equal(result.MorningWeather, createCorrectDTO.MorningWeather);
        Assert.Equal(result.MinTemperature, createCorrectDTO.MinTemperature);

        Assert.Equal(result.MaxTemperature, createCorrectDTO.MaxTemperature);
        Assert.Equal(result.WindSpeed, createCorrectDTO.WindSpeed);
        Assert.Equal(result.Humidity, createCorrectDTO.Humidity);
        Assert.Equal(result.Precipitation, createCorrectDTO.Precipitation);
    }

    [Fact(DisplayName = "Create - Verify if calls Add method in Repository, correctly")]
    public async Task Post_VerifysCommunicationWithRepository()
    {
        var result = await _service.CreateMeteorologicalData(createCorrectDTO);
        _repository.Verify(r => r.Add(It.IsAny<MeteorologicalDataEntity>()), Times.Once);
    }

    [Fact(DisplayName = "Get by Id With Valid Id - Sucess")]
    public async Task GetByID_ValidID()
    {
        //Arrange

        var weather =  new Fixture().Create<MeteorologicalDataEntity>();
         _repository.Setup(r => r.FindByID(weather.Id)).ReturnsAsync(weather);

        //Act
        var result = await _service.FindMeteorologicalDataByID(weather.Id);

        //Assert
        Assert.Equal(weather, result);
    }

    [Fact(DisplayName = "Get by Id - Verify if calls FindByID method in Repository, correctly")]
    public async Task GetByIDVerifysCommunicationWithRepository()
    {
        //Arrange
        Guid randomGuid = Guid.NewGuid();
        _repository.Setup(r => r.FindByID(randomGuid)).ReturnsAsync(new MeteorologicalDataEntity());
        //Act
        var result = await _service.FindMeteorologicalDataByID(randomGuid);
        //Assert
        _repository.Verify(r => r.FindByID(randomGuid), Times.Once);
    }

    [Fact(DisplayName = "Get by Id With Invalid Id - Failed")]
    public async Task GetByIDFailed_InvalidID()
    {
        //Arrange

        MeteorologicalDataEntity metDataWithID = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.FindByID(metDataWithID.Id)).ReturnsAsync(metDataWithID);
        var randomGuid = Guid.NewGuid();
        //Assert
        await Assert.ThrowsAsync<ServiceLayerNullPointerException>(async () => await _service.FindMeteorologicalDataByID(randomGuid));
    }

    [Fact(DisplayName = "Get by City With Valid City - Sucess")]
    public async Task GetByCityName_ValidCity()
    {
        //Arrange
        var cityName = "Porto Alegre";
        var page = 0;
        var paginatedWeathers = new Fixture().Create<PaginatedQueryWeather>();
            
        _repository.Setup(r => r.GetPaginatedDataByCity(cityName,page)).ReturnsAsync(paginatedWeathers);
        //Act
        var response = await _service.FindMeteorologicalDataByCityName(cityName, page);
        //Assert
        Assert.Equal(paginatedWeathers, response);
    }

    [Fact(DisplayName = "Get by City With Invalid City - Failed")]
    public async Task GetByCityNameFailed_InValidCity()
    {
        //Arrange
        PaginatedQueryWeather pageMocked = new Fixture().Create<PaginatedQueryWeather>();
        string cityNameWrong = "testCity";
        string cityName = "anotherCity";

        _repository.Setup(r => r.GetPaginatedDataByCity(cityName, It.IsAny<int>())).ReturnsAsync(pageMocked);
        //Assert
        await Assert.ThrowsAsync<ServiceLayerNullPointerException>(() => _service.FindMeteorologicalDataByCityName(cityNameWrong, It.IsAny<int>()));
    }

    [Fact(DisplayName = "Get by Date(Actual Day) and City - Sucess")]
    public async Task GetByActualDay()
    {
        //Arrange
        var actualDay = DateTime.Now.Date;
        var weather = new MeteorologicalDataEntity
        {
            Id = new Guid(),
            City = "Roma",
            WeatherDate = actualDay,
            MorningWeather = "Sunny",
            NightWeather = "Sunny",
            MaxTemperature = 70,
            MinTemperature = 0,
            Humidity = 20,
            WindSpeed = 20,
            Precipitation = 20
        };

        _repository.Setup(r => r.FindBySpecificDateAndCity(weather.City, weather.WeatherDate)).ReturnsAsync(weather);
        //Act
        var response =  await _service.FindActualDay(weather.City);
        //Assert
        Assert.Equal(weather, response);
        _repository.Verify(r => r.FindBySpecificDateAndCity(weather.City, actualDay), Times.Once);
    }

    [Fact(DisplayName = "Get by Date and City - Sucess")]
    public async Task GetBySpecificDate_ValidDate()
    {
        //Arrange
        var weather = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.FindBySpecificDateAndCity(weather.City, weather.WeatherDate)).ReturnsAsync(weather);
        //weather
        var response = await _service.FindMeteoroloficalDataBySpecificDate(weather.City, weather.WeatherDate);
        //Assert
        Assert.Equal(weather, response);
        _repository.Verify(r => r.FindBySpecificDateAndCity(weather.City, weather.WeatherDate), Times.Once);
    }

    [Fact(DisplayName = "Get All Meteorological datas - Sucess")]
    public async Task GetAllMeteorologicalDataSucessCall()
    {
        //Arrange
        PaginatedQueryWeather pageMock = new Fixture().Create<PaginatedQueryWeather>();
        _repository.Setup(r => r.GetPaginatedDataOfAllWeathers(It.IsAny<int>())).ReturnsAsync(pageMock);
        var skip = 0;
        //Act
        var response = await _service.FindAllMeteorologicalDataPaginated(skip);
        //Assert
        Assert.Equal(pageMock, response);
    }

    [Fact(DisplayName = "Delete by Id with Valid - Sucess ")]
    public async Task Delete_ValidId()
    {
        // Arrange
        MeteorologicalDataEntity weather = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.DeleteById(weather)).ReturnsAsync(weather);
        _repository.Setup(r => r.FindByID(weather.Id)).ReturnsAsync(weather);
        //Act
        var response = await _service.DeleteMeteorologicalData(weather.Id);
        //Assert
        Assert.Equal(weather, response);
       
    }

    [Fact(DisplayName = "Delete by Id - Verify if the repository its called just once")]
    public async Task DeleteVerifyComunnicationWithRepository()
    {
        var fakeGuid = new Guid();
        _repository.Setup(r => r.DeleteById(It.IsAny<MeteorologicalDataEntity>())).ReturnsAsync(new MeteorologicalDataEntity());
        _repository.Setup(r => r.FindByID(fakeGuid)).ReturnsAsync(new MeteorologicalDataEntity());
        var response = await _service.DeleteMeteorologicalData(It.IsAny<Guid>());
        
        _repository.Verify(r => r.DeleteById(It.IsAny<MeteorologicalDataEntity>()), Times.Once);
    }

    [Fact(DisplayName = "Delete by Id with Invalid Id - Failed")]
    public async Task DeleteFailed_InvalidId()
    {
        MeteorologicalDataEntity weather = new Fixture().Create<MeteorologicalDataEntity>();
        _repository.Setup(r => r.DeleteById(weather)).ReturnsAsync(weather);

        //Assert
        await Assert.ThrowsAsync<ServiceLayerNullPointerException>(() =>  _service.DeleteMeteorologicalData(Guid.NewGuid()));
    }


    [Fact(DisplayName = "Update by Id with Valid DTO - Sucess")]
    public async Task Put_EditWithValidIdAndValidUpdateDto()
    {
        //Arrange
        var metDataToEdit = new Fixture().Create<MeteorologicalDataEntity>();
        var editionToInsert = new Fixture().Create<MeteorologicalDataDto>();
        var randomGuid = Guid.NewGuid();

        _mapper.Setup(m => m.Map<MeteorologicalDataEntity>(editionToInsert))
     .Returns((MeteorologicalDataDto source) => new MeteorologicalDataEntity
     {
         Id = metDataToEdit.Id,
         City = source.City,
         WeatherDate = source.WeatherDate,
         MorningWeather = source.MorningWeather,
         NightWeather = source.NightWeather,
         MaxTemperature = source.MaxTemperature,
         MinTemperature = source.MinTemperature,
         Humidity = source.Humidity,
         WindSpeed = source.WindSpeed,
         Precipitation = source.Precipitation
     });
        _repository.Setup(r => r.FindByID(It.IsAny<Guid>())).ReturnsAsync(metDataToEdit);
        //Act
        var response = await _service.EditMeteorologicalData(randomGuid, editionToInsert);

        //Assert
        Assert.Equal(metDataToEdit.Id, response.Id);
        Assert.Equal(metDataToEdit.City, response.City);
        Assert.Equal(metDataToEdit.WeatherDate, response.WeatherDate);
        Assert.Equal(metDataToEdit.NightWeather, response.NightWeather);
        Assert.Equal(metDataToEdit.MorningWeather, response.MorningWeather);
        Assert.Equal(metDataToEdit.MinTemperature, response.MinTemperature);
        Assert.Equal(metDataToEdit.MaxTemperature, response.MaxTemperature);
        Assert.Equal(metDataToEdit.WindSpeed, response.WindSpeed);
        Assert.Equal(metDataToEdit.Humidity, response.Humidity);
        Assert.Equal(metDataToEdit.Precipitation, response.Precipitation);
    }

    [Fact(DisplayName = "Update - Verify if calls FindByID and Edit method in Repository, correctly ")]
    public async Task PutVerifyCommunicationWithRepository()
    {
        _repository.Setup(r => r.FindByID(It.IsAny<Guid>())).ReturnsAsync(new MeteorologicalDataEntity());
        var response = await _service.EditMeteorologicalData(Guid.NewGuid(), It.IsAny<MeteorologicalDataDto>());
        //Assert
        _repository.Verify(r => r.FindByID(It.IsAny<Guid>()), Times.Once());
        _repository.Verify(r => r.EditMeteorologicalData(), Times.Once());
    }

    [Fact(DisplayName = "Update with wrong ID - Failed")]
    public async Task Put_EditWithInvalidId()
    {
        //Arrange
        var metDataToEdit = new Fixture().Create<MeteorologicalDataEntity>();
        var editionToInsert = new Fixture().Create<MeteorologicalDataDto>();
        _mapper.Setup(m => m.Map<MeteorologicalDataEntity>(editionToInsert))
     .Returns((MeteorologicalDataDto source) => new MeteorologicalDataEntity
     {
         Id = metDataToEdit.Id,
         City = source.City,
         WeatherDate = source.WeatherDate,
         MorningWeather = source.MorningWeather,
         NightWeather = source.NightWeather,
         MaxTemperature = source.MaxTemperature,
         MinTemperature = source.MinTemperature,
         Humidity = source.Humidity,
         WindSpeed = source.WindSpeed,
         Precipitation = source.Precipitation
     });
        _repository.Setup(r => r.FindByID(metDataToEdit.Id)).ReturnsAsync(metDataToEdit);
        //Assert
        await Assert.ThrowsAsync<ServiceLayerNullPointerException>(() => _service.EditMeteorologicalData(Guid.NewGuid(), editionToInsert));
    }

    [Fact(DisplayName = "Update only one field with Valid Jason PatchDocument - Sucess")]
    public async Task Patch_EditWithValidIdAndValidJsonPatch()
    {
        //Arrange
        var jsonPatchDocument = new JsonPatchDocument<MeteorologicalDataDto>();
        jsonPatchDocument.Operations.Add(new Operation<MeteorologicalDataDto>("replace", "/City", null, "Jerusalem"));
        var metDataToEdit = new Fixture().Create<MeteorologicalDataEntity>();
        _mapper.Setup(m => m.Map<MeteorologicalDataDto>(metDataToEdit))
            .Returns(() => new MeteorologicalDataDto
            {
                City = metDataToEdit.City,
                WeatherDate = metDataToEdit.WeatherDate,
                MorningWeather = metDataToEdit.MorningWeather,
                NightWeather = metDataToEdit.NightWeather,
                MaxTemperature = metDataToEdit.MaxTemperature,
                MinTemperature = metDataToEdit.MinTemperature,
                Humidity = metDataToEdit.Humidity,
                WindSpeed = metDataToEdit.WindSpeed,
                Precipitation = metDataToEdit.Precipitation
            });

        var metDataAlreadyEdited = new MeteorologicalDataEntity
        {
            City = "Jerusalem",
            WeatherDate = metDataToEdit.WeatherDate,
            MorningWeather = metDataToEdit.MorningWeather,
            NightWeather = metDataToEdit.NightWeather,
            MaxTemperature = metDataToEdit.MaxTemperature,
            MinTemperature = metDataToEdit.MinTemperature,
            Humidity = metDataToEdit.Humidity,
            WindSpeed = metDataToEdit.WindSpeed,
            Precipitation = metDataToEdit.Precipitation

        };
        _mapper.Setup(m => m.Map
        (It.IsAny<MeteorologicalDataDto>(), It.IsAny<MeteorologicalDataEntity>()))
            .Returns(() => new MeteorologicalDataEntity
            {
                Id = metDataToEdit.Id,
                City = metDataAlreadyEdited.City,
                WeatherDate = metDataAlreadyEdited.WeatherDate,
                MorningWeather = metDataAlreadyEdited.MorningWeather,
                NightWeather = metDataAlreadyEdited.NightWeather,
                MaxTemperature = metDataAlreadyEdited.MaxTemperature,
                MinTemperature = metDataAlreadyEdited.MinTemperature,
                Humidity = metDataAlreadyEdited.Humidity,
                WindSpeed = metDataAlreadyEdited.WindSpeed,
                Precipitation = metDataAlreadyEdited.Precipitation
            });
        _repository.Setup(r => r.FindByID(It.IsAny<Guid>())).ReturnsAsync(metDataToEdit);
        //Act
        var response = await _service.EditOnlyOneField(metDataToEdit.Id, jsonPatchDocument);
        //Assert
        Assert.NotNull(response);
        Assert.Equal(metDataToEdit.Id, response.Id);
        Assert.Equal(metDataAlreadyEdited.City, response.City);
        Assert.Equal(metDataAlreadyEdited.WeatherDate, response.WeatherDate);
    }


}
