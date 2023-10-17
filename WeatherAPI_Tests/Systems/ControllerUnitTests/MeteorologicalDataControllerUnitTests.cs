using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using WeatherAPI_DOTNET.Controllers;
using WeatherAPI_DOTNET.Data.Repository;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service;
using AutoFixture;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI_DOTNET.Service.Interfaces;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using WeatherAPI_DOTNET.Data.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WeatherApi.DotNet.Tests.Helpers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;

namespace WeatherApi.DotNet.Tests.Systems.ControllerUnitTests;


public class MeteorologicalDataControllerUnitTests
{
    private Mock<IMeteorologicalDataRepository> _repository;
    private Mock<IMeteorologicalDataService> _service;
    private Mock<IMapper> _mapper;
    private readonly MeteorologicalDataController _controller;
    private readonly ControllerTestsHelper _helper;

    public MeteorologicalDataControllerUnitTests()
    {
        _repository = new Mock<IMeteorologicalDataRepository>();
        _service = new Mock<IMeteorologicalDataService>();
        _mapper = new Mock<IMapper>();
        _controller = new MeteorologicalDataController(_service.Object);
        _helper = new ControllerTestsHelper();
    }

    [Fact]
    public void GetAllTestSucessefulOkResponse()
    {
        //Arrange
        var skip = 0;
        var meteorologicalDataList = new List<MeteorologicalDataEntity>
        {
            new Fixture().Create<MeteorologicalDataEntity>() ,
            new Fixture().Create<MeteorologicalDataEntity>(),
        };

        _service.Setup(s => s.FindAllMeteorologicalDataPaginated(skip)).Returns(meteorologicalDataList);
        //Act
        var response = _controller.GetAll(skip);
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<MeteorologicalDataEntity>>(okResult.Value);
        Assert.Equal(meteorologicalDataList, returnedData);
    }

    [Fact]
    public async Task GetAllTestFailedNoContentResponse()
    {
        //Arrange
        var skip = 0;
        var meteorologicalDataList = new List<MeteorologicalDataEntity>
        {
        };

        _service.Setup(s => s.FindAllMeteorologicalDataPaginated(skip)).Returns(meteorologicalDataList);
        //Act
        var response = _controller.GetAll(skip);
        //Assert
        var noContentResult = Assert.IsType<NoContentResult>(response);
        Assert.Equal(noContentResult, response);
    }

    [Fact]
    public void GetAllVerifyIfCall()
    {
        //Act
        var response = _controller.GetAll(It.IsAny<int>());
        //Assert
        _service.Verify(s => s.FindAllMeteorologicalDataPaginated(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void GetByID()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.FindMeteorologicalDataByID(guid)).Returns(mockedMetData);
        //Act
        var response = _controller.FindMeteorologicalDataByID(guid);
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(mockedMetData, returnedData);
    }

    [Fact]
    public void GetByIdInexistentIDFaield()
    {
        var wrongGuid = Guid.NewGuid();
        var response = _controller.FindMeteorologicalDataByID(wrongGuid);
        var NotFoundObjectResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal("Meteorological Data not Found", NotFoundObjectResult.Value);
    }

    [Fact]
    public void PostNewMeteorologicalData()
    {
        var mockedCreateDTO = new Fixture().Create<CreateMetDataDto>();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.CreateMeteorologicalData(mockedCreateDTO)).Returns(mockedMetData);

        var response = _controller.CreateMeteorologicalData(mockedCreateDTO);

        var createdResult = Assert.IsType<CreatedAtActionResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(createdResult.Value);
        Assert.Equal(mockedMetData, returnedData);
        _service.Verify(s => s.CreateMeteorologicalData(It.IsAny<CreateMetDataDto>()), Times.Once());
    }


    [Fact]
    public void PostWithInvalidDTOFailed()
    {
        //Esse é a solução mais fácil, porém não a mais recomendável
        //_controller.ModelState.AddModelError("fakeError", "InvalidModel");
        //var response = _controller.CreateMeteorologicalData(new());
        //Assert.IsType<BadRequestObjectResult>(response);
        var wrongDTO = new CreateMetDataDto
        {
            City = "Roma",
            WeatherDate = new DateTime(2023, 2, 3),
            MorningWeather = "Sunny",
            NightWeather = "Sunny",
            MaxTemperature = 70,
            MinTemperature = 5,
            Humidity = 20,
            WindSpeed = 1000,
            Precipitation = 20
        };

        _helper.MockModelState(wrongDTO, _controller);
        var response = _controller.CreateMeteorologicalData(wrongDTO);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public void DeleteByIdSucessWithRightID()
    {
        //Arrange
        var guid = new Guid();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.DeleteMeteorologicalData(guid)).Returns(mockedMetData);
        //act
        var response = _controller.DeleteMeteorologicalDataByID(guid);
        //assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal("Deleted with Sucess!", okResult.Value);
    }

    [Fact]
    public void DeleteByIdFailedWithWrongID()
    {
        var response = _controller.DeleteMeteorologicalDataByID(It.IsAny<Guid>());
        var notFound = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal("Id not found.", notFound.Value);
    }
    [Fact]
    public void DeleteVerifyComunicationWithService()
    {
        var response = _controller.DeleteMeteorologicalDataByID(It.IsAny<Guid>());
        _service.Verify(s => s.DeleteMeteorologicalData(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void GetByCityWithExistingCitySucess()
    {
        //Arrange
        var cityName = "Porto Alegre";
        var metDataList = new Fixture().Create<IEnumerable<MeteorologicalDataEntity>>();
        _service.Setup(s => s.FindMeteorologicalDataByCityName(cityName)).Returns(metDataList);
        //Act
        var response = _controller.FindMeteorologicalDataByCity(cityName);
        //Assert
        var okResult = Assert.IsAssignableFrom<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<MeteorologicalDataEntity>>(okResult.Value);
        Assert.Equal(metDataList, returnedData);
    }

    [Fact]
    public void GetByCityVerifyComunication()
    {
        _controller.FindMeteorologicalDataByCity(It.IsAny<String>());
        _service.Verify(s => s.FindMeteorologicalDataByCityName(It.IsAny<String>()), Times.Once);
    }
    [Fact]
    public void GetByCityWithWrongCityFailed()
    {
        var cityName = "Porto Alegre";
        var response = _controller.FindMeteorologicalDataByCity(cityName);
        var notFoundResult = Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<String>(notFoundResult.Value);
        Assert.Equal("There is no Meteorological Data found with this City", returnedData);
    }

    [Fact]
    public void GetByActualDayWithSucess()
    {
        var actualDay = DateTime.Now.Date;
        var metData = new MeteorologicalDataEntity
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
        _service.Setup(s => s.FindActualDay(metData.City)).Returns(metData);

        var response = _controller.FindActualDayInCity(metData.City);

        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metData, returnedData);
    }

    [Fact]
    public void GetByActualDayVerifyComunication()
    {
        _controller.FindActualDayInCity(It.IsAny<String>());
        _service.Verify(s => s.FindActualDay(It.IsAny<String>()), Times.Once);
    }

    [Fact]
    public void GetByActualDayFailed()
    {
        var cityName = "Porto Alegre";
        var response = _controller.FindActualDayInCity(cityName);
        var notFound = Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<String>(notFound.Value);
        Assert.Equal("There is no today's Meteorological Data found with this City.", returnedData);
    }

    [Fact]
    public void EditByIdWithSucess()
    {
        var guid = new Guid();
        var editionDto = new Fixture().Create<UpdateMetDataDto>();
        var metDataReturned = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.EditMeteorologicalData(guid, editionDto)).Returns(metDataReturned);
        var response = _controller.UpdateMeteorologicalDataById(guid, editionDto);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metDataReturned, returnedData);
    }


    [Fact]
    public void EditByIdFailedWrongID()
    {
        var guid = new Guid();
        var editionDto = new Fixture().Create<UpdateMetDataDto>();
        var response = _controller.UpdateMeteorologicalDataById(guid, editionDto);
        var notFound = Assert.IsType<NotFoundObjectResult>(response);
        var ReturnedData = Assert.IsAssignableFrom<String>(notFound.Value);
        Assert.Equal("Id not found", ReturnedData);
    }
    [Fact]
    public void GetSpecificDateInCitySucess()
    {
        var metData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.FindMeteoroloficalDataBySpecificDate(metData.City, metData.WeatherDate)).Returns(metData);
        var response = _controller.FindSpecificDateInCity(metData.City, metData.WeatherDate);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsType<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metData, returnedData);
    }

    [Fact]
    public void GetSpecificDateInCityFailed()
    {
        var metData = new Fixture().Create<MeteorologicalDataEntity>();
        var response = _controller.FindSpecificDateInCity(metData.City, metData.WeatherDate);
        var notFound = Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public void EditEspecificFieldByIDSucess()
    {
        var guid = new Guid();
        var jsonPatchDocument = new JsonPatchDocument<UpdateMetDataDto>();
        jsonPatchDocument.Operations.Add(new Operation<UpdateMetDataDto>("replace", "/City", null, "Jerusalem"));
        var metDataToEdit = new Fixture().Create<MeteorologicalDataEntity>();
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
        _service.Setup(s => s.EditOnlyOneField(guid, jsonPatchDocument)).Returns(metDataAlreadyEdited);
        var response = _controller.ParcialEditMeteorologicalDataByID(guid, jsonPatchDocument);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metDataAlreadyEdited, returnedData);

    }
    [Fact]
    public void EditEspecificFieldByIDFailedWrongID()
    {
        var guid = Guid.NewGuid();
        var jsonPatchDocument = new JsonPatchDocument<UpdateMetDataDto>();
        jsonPatchDocument.Operations.Add(new Operation<UpdateMetDataDto>("replace", "/City", null, "Jerusalem"));
        var response = _controller.ParcialEditMeteorologicalDataByID(guid, jsonPatchDocument);
        var notFound = Assert.IsType<NotFoundObjectResult>(response);
        var ReturnedData = Assert.IsAssignableFrom<String>(notFound.Value);
        Assert.Equal("Id not found", ReturnedData);
    }

   
}