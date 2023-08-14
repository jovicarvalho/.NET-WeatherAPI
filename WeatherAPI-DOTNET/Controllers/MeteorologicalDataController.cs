using Microsoft.AspNetCore.Mvc;
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
    public MeteorologicalDataController(MeteorologicalDataContext context, IMapper mapper, IMeteorologicalDataService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    [HttpPost]
    public IActionResult CreateMeteorologicalData([FromBody] CreateMetDataDto metDataDto)
    {
        MeteorologicalDataEntity metDataEntity = _mapper.Map<MeteorologicalDataEntity>(metDataDto);
        _context.MeteorologicalData.Add(metDataEntity);
        _context.SaveChanges();
        return CreatedAtAction(nameof(FindMeteorologicalDataByID), new { id = metDataEntity.Id },
            metDataEntity);
    }

    [HttpGet]
    public IEnumerable<MeteorologicalDataEntity> GetAll([FromQuery] int skip = 0)
    {
        return _context.MeteorologicalData.Skip(0).Take(10);
    }

    [HttpGet("{id}")]
    public IActionResult FindMeteorologicalDataByID(int id)
    {
        var metData = _service.findMeteorologicalDataByID(id);
        if (metData == null) { return NotFound("Meteorological Data not Found");}
        return Ok(metData);
    }

    [HttpGet("city={cityName}")]
    public IActionResult FindMeteorologicalDataByCity (string cityName)
    {
        List<MeteorologicalDataEntity> metDataList = _service.FindMeteorologicalDataByCityName(cityName);
        if (metDataList.Count == 0) return NotFound();
        return Ok(metDataList);
    }

    [HttpGet("actualDay/city={cityName}")]
    public IActionResult FindActualDayInCity(string cityName)
    {
        var date = DateTime.Now;
        List<MeteorologicalDataEntity> metDataList = _service.FindMeteorologicalDataByCityName(cityName);
        if (metDataList.Count == 0) return NotFound("This city doesn't have a WeatherData.");
        MeteorologicalDataEntity actualDay = _service.FindMeteoroloficalDataByActualDay(date, metDataList);
        if (actualDay == null) return NoContent();
        return Ok(actualDay);
    }
    [HttpGet("specificDate/city={cityName}")]
    public IActionResult FindSpecificDateInCity(string cityName, [FromBody] DateTime date)
    {
        List<MeteorologicalDataEntity> metDataList = _service.FindMeteorologicalDataByCityName(cityName);
        if (metDataList.Count == 0) return NotFound("This city doesn't have a WeatherDate.");
        Console.WriteLine(date);
        MeteorologicalDataEntity actualDay = _service.FindMeteoroloficalDataByActualDay(date, metDataList);
        return Ok(actualDay);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMeteorologicalDataById
        (
        int id,
        [FromBody] UpdateMetDataDto metDataDto
        )
    {
        var metData = _service.findMeteorologicalDataByID(id);
        if (metData == null) return NotFound();
        _mapper.Map(metDataDto, metData);
        _context.SaveChanges();
        return Ok(metDataDto);
    }
  

    [HttpPatch("{id}")]
    public IActionResult ParcialEditMeteorologicalDataByID
        (
        int id, 
        [FromBody] JsonPatchDocument<UpdateMetDataDto> patch
        ) 
    {
             var metData = _service.findMeteorologicalDataByID(id);

        if (metData == null) return NotFound();

        var metDataAtuazalizar = _mapper.Map<UpdateMetDataDto>(metData);

        patch.ApplyTo(metDataAtuazalizar, ModelState);
        if (!TryValidateModel(metDataAtuazalizar)) return ValidationProblem(ModelState);

        _mapper.Map(metDataAtuazalizar, metData);    
        _context.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeteorologicalDataByID( int id )
    {
        var metData = _context.MeteorologicalData.FirstOrDefault(metData => metData.Id == id);
        if (metData == null) return NotFound();
        _context.Remove(metData);
        _context.SaveChanges();
        return Ok("Deleted with Sucess");
    }

}
