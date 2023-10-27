using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using System.Security.Cryptography;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using WeatherApi.DotNet.Domain.Dtos;

namespace WeatherApi.DotNet.Tests.Systems.ControllerUnitTests;


public class MeteorologicalDataControllerUnitTests
{
    private readonly Mock<IMeteorologicalDataService> _service;
    private readonly MeteorologicalDataController _controller;

    public MeteorologicalDataControllerUnitTests()
    {
        _service = new Mock<IMeteorologicalDataService>();
        _controller = new MeteorologicalDataController(_service.Object);
    }

    [Fact]
    public async Task GetAllTestSucessefulOkResponse()
    {
        //Arrange
        var skip = 0;
        var paginatedWeathers = new Fixture().Create<PaginatedQueryWeather>();

        _service.Setup(s => s.FindAllMeteorologicalDataPaginated(skip)).ReturnsAsync(paginatedWeathers);
        //Act
        var response = await _controller.GetAllPagineted(skip);
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<PaginatedQueryWeather>(okResult.Value);
        Assert.Equal(paginatedWeathers, returnedData);
    }

    [Fact]
    public async Task GetAllPaginetedVerifyCommunicationWithService()
    {
        //Act
        var response = await _controller.GetAllPagineted(It.IsAny<int>());
        //Assert
        _service.Verify(s => s.FindAllMeteorologicalDataPaginated(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetByIDWithSucess()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var mockedWeather = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.FindMeteorologicalDataByID(guid)).ReturnsAsync(mockedWeather);
        //Act
        var response = await _controller.FindMeteorologicalDataByID(guid);
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(mockedWeather, returnedData);
    }

    [Fact]
    public async Task PostNewMeteorologicalData()
    {
        var mockedCreateDTO = new Fixture().Create<MeteorologicalDataDto>();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.CreateMeteorologicalData(mockedCreateDTO)).ReturnsAsync(mockedMetData);

        var response = await _controller.CreateMeteorologicalData(mockedCreateDTO);

        var createdResult = Assert.IsType<CreatedAtActionResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(createdResult.Value);
        Assert.Equal(mockedMetData, returnedData);
        _service.Verify(s => s.CreateMeteorologicalData(It.IsAny<MeteorologicalDataDto>()), Times.Once());
    }


    [Fact]
    public async Task DeleteByIdSucessWithRightID()
    {
        //Arrange
        var guid = new Guid();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.DeleteMeteorologicalData(guid)).ReturnsAsync(mockedMetData);
        //act
        var response = await _controller.DeleteMeteorologicalDataByID(guid);
        //assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal("Deleted with Sucess!", okResult.Value);
    }

    [Fact]
    public async Task DeleteVerifyComunicationWithService()
    {
        var response = await _controller.DeleteMeteorologicalDataByID(It.IsAny<Guid>());
        _service.Verify(s => s.DeleteMeteorologicalData(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetByCityWithExistingCitySucess()
    {
        //Arrange
        var cityName = "Porto Alegre";
        int page = 0;
        var paginetedWeathers = new Fixture().Create<PaginatedQueryWeather>();
        _service.Setup(s => s.FindMeteorologicalDataByCityName(cityName, page)).ReturnsAsync(paginetedWeathers);
        //Act
        var response = await _controller.FindMeteorologicalDataByCity(cityName, page);
        //Assert
        var okResult = Assert.IsAssignableFrom<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<PaginatedQueryWeather> (okResult.Value);
        Assert.Equal(paginetedWeathers, returnedData);
    }

    [Fact]
    public async Task GetByCityVerifyComunication()
    {
        await _controller.FindMeteorologicalDataByCity(It.IsAny<String>(),It.IsAny<int>());
        _service.Verify(s => s.FindMeteorologicalDataByCityName(It.IsAny<String>(), It.IsAny<int>()), Times.Once);
    }


    [Fact]
    public async Task GetByActualDayWithSucess()
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
        _service.Setup(s => s.FindActualDay(metData.City)).ReturnsAsync(metData);

        var response = await _controller.FindActualDayInCity(metData.City);

        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metData, returnedData);
    }

    [Fact]
    public async Task GetByActualDayVerifyComunication()
    {
        await _controller.FindActualDayInCity(It.IsAny<String>());
        _service.Verify(s => s.FindActualDay(It.IsAny<String>()), Times.Once);
    }


    [Fact]
    public async Task EditByIdWithSucess()
    {
        var guid = new Guid();
        var editionDto = new Fixture().Create<MeteorologicalDataDto>();
        var metDataReturned = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.EditMeteorologicalData(guid, editionDto)).ReturnsAsync(metDataReturned);
        var response = await _controller.UpdateMeteorologicalDataById(guid, editionDto);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metDataReturned, returnedData);
    }



    [Fact]
    public async Task GetSpecificDateInCitySucess()
    {
        var metData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s => s.FindMeteoroloficalDataBySpecificDate(metData.City, metData.WeatherDate)).ReturnsAsync(metData);
        var response = await _controller.FindSpecificDateInCity(metData.City, metData.WeatherDate);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsType<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metData, returnedData);
    }


    [Fact]
    public async Task EditEspecificFieldByIDSucess()
    {
        var guid = new Guid();
        var jsonPatchDocument = new JsonPatchDocument<MeteorologicalDataDto>();
        jsonPatchDocument.Operations.Add(new Operation<MeteorologicalDataDto>("replace", "/City", null, "Jerusalem"));
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
        _service.Setup(s => s.EditOnlyOneField(guid, jsonPatchDocument)).ReturnsAsync(metDataAlreadyEdited);
        var response = await _controller.ParcialEditMeteorologicalDataByID(guid, jsonPatchDocument);
        var okResult = Assert.IsType<OkObjectResult>(response);
        var returnedData = Assert.IsAssignableFrom<MeteorologicalDataEntity>(okResult.Value);
        Assert.Equal(metDataAlreadyEdited, returnedData);

    }


}