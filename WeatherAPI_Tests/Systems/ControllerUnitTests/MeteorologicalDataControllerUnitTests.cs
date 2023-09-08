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

namespace WeatherApi.DotNet.Tests.Systems.ControllerUnitTests;


public class MeteorologicalDataControllerUnitTests
{
    private Mock<IMeteorologicalDataRepository> _repository;
    private Mock<IMeteorologicalDataService> _service;
    private Mock<IMapper> _mapper;
    private readonly MeteorologicalDataController _controller;

    public MeteorologicalDataControllerUnitTests()
    {
        _repository = new Mock<IMeteorologicalDataRepository>();
        _service = new Mock<IMeteorologicalDataService>();
        _mapper = new Mock<IMapper>();
        _controller = new MeteorologicalDataController(_service.Object);
    }

    [Fact]
    public async Task GetAllTestSucessefulOkResponse()
    {
        //Arrange
        var skip = 0;
        var meteorologicalDataList = new List<MeteorologicalDataEntity>
        {
            new Fixture().Create<MeteorologicalDataEntity>() ,
            new Fixture().Create<MeteorologicalDataEntity>(),
        };

        _service.Setup(s=>s.FindAllMeteorologicalData(skip)).Returns(meteorologicalDataList);
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

        _service.Setup(s => s.FindAllMeteorologicalData(skip)).Returns(meteorologicalDataList);
        //Act
        var response = _controller.GetAll(skip);
        //Assert
        var noContentResult = Assert.IsType<NoContentResult>(response);
        Assert.Equal(noContentResult, response);
    }

    [Fact]
    public async Task GetAllVerifyIfCall()
    {
        //Act
        var response = _controller.GetAll(It.IsAny<int>());
        //Assert
        _service.Verify(s => s.FindAllMeteorologicalData(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetByID()
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
    public  async Task GetByIdInexistentIDFaield()
    {
        var wrongGuid = Guid.NewGuid();
        var response = _controller.FindMeteorologicalDataByID(wrongGuid);
        var NotFoundObjectResult =  Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal("Meteorological Data not Found", NotFoundObjectResult.Value);
    }

    [Fact]
    public async Task PostNewMeteorologicalData()
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


   //[Fact]
    //public async Task PostWithInvalidDTOFailed()
    //{
    //    var wrongDTO = new CreateMetDataDto
    //    {
    //        City = "Roma",
    //        WeatherDate = new DateTime(2023, 2, 3),
    //        MorningWeather = "Sunny",
    //        NightWeather = "Sunny",
    //        MaxTemperature = 70,
    //        MinTemperature = 5,
    //        Humidity = 20,
    //        WindSpeed = 1000,
    //        Precipitation = 20
    //    };
    //   // var wrongDTO = new CreateMetDataDto();
    //    var response = _controller.CreateMeteorologicalData(wrongDTO);
    //    Assert.IsType<BadRequestObjectResult>(response);
    //}

    [Fact]
    public async Task DeleteByIdSucessWithRightID()
    {
        //Arrange
        var guid = new Guid();
        var mockedMetData = new Fixture().Create<MeteorologicalDataEntity>();
        _service.Setup(s=>s.DeleteMeteorologicalData(guid)).Returns(mockedMetData);
        //act
        var response = _controller.DeleteMeteorologicalDataByID(guid);
        //assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal("Deleted with Sucess!", okResult.Value);
    }

    [Fact]
    public async Task DeleteByIdFailedWithWrongID()
    {
        var response = _controller.DeleteMeteorologicalDataByID(It.IsAny<Guid>());
        var notFound = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal("Id not found.", notFound.Value);
    }
    [Fact]
    public async Task DeleteVerifyComunicationWithService()
    {
        var response = _controller.DeleteMeteorologicalDataByID(It.IsAny<Guid>());
        _service.Verify(s => s.DeleteMeteorologicalData(It.IsAny<Guid>()), Times.Once);
    }

    //[Fact]
    //public async Task 


}
