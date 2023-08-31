﻿using Microsoft.AspNetCore.Mvc;
using WeatherAPI_DOTNET.Models;
using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Service.Interfaces;


namespace WeatherAPI_DOTNET.Controllers;

[ApiController]
[Route("[controller]")]
public class MeteorologicalDataController: ControllerBase
{
    
    private IMeteorologicalDataService _service;
    public MeteorologicalDataController(IMeteorologicalDataService service)
    {
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
    public IActionResult FindMeteorologicalDataByID(Guid id)
    {
        var metData = _service.FindMeteorologicalDataByID(id);
        return metData is null ? NotFound("Meteorological Data not Found") : Ok(metData);
    }

    [HttpGet("city={cityName}")]
    public IActionResult FindMeteorologicalDataByCity(string cityName)
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
        Guid id,
        [FromBody] UpdateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metDataEdited = _service.EditMeteorologicalData(id, metDataDto);
        return metDataEdited is null ? NotFound("Id not found") : Ok(metDataEdited);
    }


    [HttpPatch("{id}")]
    public IActionResult ParcialEditMeteorologicalDataByID(
        Guid id,
        [FromBody] JsonPatchDocument<UpdateMetDataDto> patch)
    {
        var metDataEdited = _service.EditOnlyOneField(id, patch);
        Console.WriteLine(patch);
        return Ok(metDataEdited);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeteorologicalDataByID(Guid id)
    {
        return _service.DeleteMeteorologicalData(id) is null ? NotFound("Id not found.") : Ok("Deleted with Sucess!");
    }

}
