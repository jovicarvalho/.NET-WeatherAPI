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
    
    private readonly IMeteorologicalDataService _service;
    public MeteorologicalDataController(IMeteorologicalDataService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeteorologicalData([FromBody] MeteorologicalDataDto metDataDto)
    {
        var metDataEntity = await _service.CreateMeteorologicalData(metDataDto);
        return CreatedAtAction(nameof(FindMeteorologicalDataByID), new { id = metDataEntity.Id },
            metDataEntity);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllPagineted([FromQuery] int skip)
    {   
        var paginatedQueryofAllWeathers = await _service.FindAllMeteorologicalDataPaginated(skip);
        return Ok(paginatedQueryofAllWeathers);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> FindMeteorologicalDataByID(Guid id)    
    {
        var metData = await _service.FindMeteorologicalDataByID(id);
        return Ok(metData);
    }

    [HttpGet]
    public async Task<IActionResult> FindMeteorologicalDataByCity([FromQuery] string city, [FromQuery] int page)
    {
        var metDataList = await _service.FindMeteorologicalDataByCityName(city, page);
        return Ok(metDataList);
    }

    [HttpGet("actualDay/")]
    public async Task<IActionResult> FindActualDayInCity([FromQuery] string cityName)
    {
        MeteorologicalDataEntity actualDay = await _service.FindActualDay(cityName);
        return Ok(actualDay);
    }

    [HttpGet("weekInCity/")]
    public async Task<IActionResult> FindSevenDaysInCity([FromQuery] string cityName)
    {
        var weekinCity = await _service.FindWeekInCity(cityName);
        return Ok(weekinCity);
    }

    [HttpGet("specificDate/cityName")]
    public async Task<IActionResult> FindSpecificDateInCity([FromQuery] string cityName, [FromBody] DateTime date)
    {
        MeteorologicalDataEntity specificDate = await _service.FindMeteoroloficalDataBySpecificDate(cityName, date);
        return Ok(specificDate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeteorologicalDataById(
        Guid id,
        [FromBody] MeteorologicalDataDto metDataDto)
    {
        MeteorologicalDataEntity metDataEdited = await _service.EditMeteorologicalData(id, metDataDto);
        return Ok(metDataEdited);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> ParcialEditMeteorologicalDataByID(
        Guid id,
        [FromBody] JsonPatchDocument<MeteorologicalDataDto> patch)
    {
        var metDataEdited = await _service.EditOnlyOneField(id, patch);
        return Ok(metDataEdited);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeteorologicalDataByID(Guid id)
    {
        await _service.DeleteMeteorologicalData(id);
        return Ok("Deleted with Sucess!");
    }

}
