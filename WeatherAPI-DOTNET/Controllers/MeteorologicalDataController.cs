using Microsoft.AspNetCore.Mvc;
using WeatherAPI_DOTNET.Models;
using Microsoft.AspNetCore.JsonPatch;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Service.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace WeatherAPI_DOTNET.Controllers;

[ApiController]
[Route("[controller]")]
[EnableCors("WeatherFront")]
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
        if(!ModelState.IsValid) return BadRequest(ModelState);
        MeteorologicalDataEntity metDataEntity = _service.CreateMeteorologicalData(metDataDto);
        return CreatedAtAction(nameof(FindMeteorologicalDataByID), new { id = metDataEntity.Id },
            metDataEntity);
    }

    [HttpGet("all")]
    public IActionResult GetAllPagineted([FromQuery] int skip)
    {   
        var paginatedQueryofAllWeathers = _service.FindAllMeteorologicalDataPaginated(skip);
        return (paginatedQueryofAllWeathers.totalWeathers != 0) ? Ok(paginatedQueryofAllWeathers) : NotFound("There is no MeteorologicalData registred ");
    }


    [HttpGet("{id}")]
    public IActionResult FindMeteorologicalDataByID(Guid id)    
    {
        var metData = _service.FindMeteorologicalDataByID(id);
        return metData is null ? NotFound("Meteorological Data not Found") : Ok(metData);
    }

    [HttpGet]
    public IActionResult FindMeteorologicalDataByCity([FromQuery] string city, [FromQuery] int page)
    {
        var metDataList = _service.FindMeteorologicalDataByCityName(city, page);
        return (metDataList.totalWeathers != 0) ?  Ok(metDataList) : NotFound("There is no Meteorological Data found with this City");
    }

    [HttpGet("actualDay/")]
    public IActionResult FindActualDayInCity([FromQuery] string cityName)
    {
        MeteorologicalDataEntity actualDay = _service.FindActualDay(cityName);
        return actualDay is null ? NotFound("There is no today's Meteorological Data found with this City.") : Ok(actualDay);
    }

    [HttpGet("weekInCity/")]
    public IActionResult FindSevenDaysInCity([FromQuery] string cityName)
    {
        IEnumerable<MeteorologicalDataEntity>actualDay = _service.findWeekInCity(cityName);
        return actualDay is null ? NotFound("There is no today's Meteorological Data found with this City.") : Ok(actualDay);
    }

    //transformar cityName em query e não um literal para não interferir na analise de performace
    [HttpGet("specificDate/cityName")]
    public IActionResult FindSpecificDateInCity([FromQuery] string cityName, [FromBody] DateTime date)
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
        return metDataEdited is null ? NotFound("Id not found") : Ok(metDataEdited);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeteorologicalDataByID(Guid id)
    {
        return _service.DeleteMeteorologicalData(id) is null ? NotFound("Id not found.") : Ok("Deleted with Sucess!");
    }

}
