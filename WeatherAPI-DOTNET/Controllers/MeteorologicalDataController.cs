using Microsoft.AspNetCore.Mvc;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Controllers;

[ApiController]
[Route("[controller]")]
public class MeteorologicalDataController: ControllerBase
{
    private MeteorologicalDataContext _context;
    public MeteorologicalDataController(MeteorologicalDataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateMeteorologicalData([FromBody] MeteorologicalDataEntity metData)
    {
        _context.MeteologicalData.Add(metData);
        _context.SaveChanges();
        return Ok(metData);
    }

    [HttpGet]
    public IEnumerable<MeteorologicalDataEntity> GetAll([FromQuery] int skip = 0)
    {
        return _context.MeteologicalData.Skip(0).Take(10);
    }

    [HttpGet("{id}")]
    public IActionResult FindMeteorologicalDataByID(int id)
    {
        var metData = _context.MeteologicalData.FirstOrDefault(metData => metData.Id == id);
        if (metData == null) { return NotFound();}
        return Ok(metData);
    }

    [HttpGet("city={cityName}")]
    public IActionResult FindMeteorologicalDataByCity (string cityName)
    {
        List<MeteorologicalDataEntity> metDataList = _context.MeteologicalData.Where(metDataList => metDataList.City == cityName).ToList();
        if(metDataList.Count == 0)
        {
            return NotFound();
        }
        return Ok(metDataList);
    }
}
