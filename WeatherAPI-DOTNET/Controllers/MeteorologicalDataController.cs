﻿using Microsoft.AspNetCore.Mvc;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Models;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Service.Interfaces;
using WeatherAPI_DOTNET.Service;

namespace WeatherAPI_DOTNET.Controllers;

[ApiController]
[Route("[controller]")]
public class MeteorologicalDataController: ControllerBase
{
    private MeteorologicalDataContext _context;
    private IMapper _mapper;
    private IMeteorologicalDataService _service;
    public MeteorologicalDataController(
        MeteorologicalDataContext context,
        IMapper mapper, 
        IMeteorologicalDataService service
        )
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    [HttpPost]
    public IActionResult CreateMeteorologicalData([FromBody] CreateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metDataEntity = _service.CreateMeteorologicalData(metDataDto);
        return CreatedAtAction(nameof(FindMeteorologicalDataByID), new { id = metDataEntity.Id },
            metDataEntity);
    }

    [HttpGet]
    public IEnumerable<MeteorologicalDataEntity> GetAll([FromQuery] int skip)
    {
        return _service.FindAllMeteorologicalData((skip));
    }


    [HttpGet("{id}")]
    public IActionResult FindMeteorologicalDataByID(int id)
    {
        var metData = _service.FindMeteorologicalDataByID(id);
        return metData is null ? NotFound("Meteorological Data not Found") : Ok(metData);
    }

    [HttpGet("city={cityName}")]
    public IActionResult FindMeteorologicalDataByCity (string cityName)
    {
       IEnumerable<MeteorologicalDataEntity> metDataList = _service.FindMeteorologicalDataByCityName(cityName);
       return metDataList.Any() ? Ok(metDataList) : NotFound("There is no Meteorological Data found with this City");
    }

    [HttpGet("actualDay/city={cityName}")]
    public IActionResult FindActualDayInCity(string cityName)
    {
        MeteorologicalDataEntity actualDay = _service.FindActualDay(cityName);
        return actualDay is null ? NotFound() : Ok(actualDay);
    }


    [HttpGet("specificDate/city={cityName}")]
    public IActionResult FindSpecificDateInCity(string cityName, [FromBody] DateTime date)
    {
        MeteorologicalDataEntity especificDate = _service.FindMeteoroloficalDataBySpecificDate(cityName, date);
        return especificDate is null ? NotFound() : Ok(especificDate);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMeteorologicalDataById(
        int id,
        [FromBody] UpdateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metDataEdited = _service.EditMeteorologicalData(id, metDataDto);
        return metDataEdited is null ? NotFound("Id not found") : Ok(metDataEdited);
    }


    [HttpPatch("{id}")]
    public IActionResult ParcialEditMeteorologicalDataByID(
        int id,
        [FromBody] JsonPatchDocument<UpdateMetDataDto> patch) 
    {
        var metDataEdited = _service.EditOnlyOneField(id, patch);
        Console.WriteLine(patch);
        return Ok(metDataEdited);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeteorologicalDataByID(int id )
    {
        return _service.DeleteMeteorologicalData(id) is null ? NotFound("Id not found.") : Ok("Deleted with Sucess!");
    }

}
