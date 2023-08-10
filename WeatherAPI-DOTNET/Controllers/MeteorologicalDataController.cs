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
        return Ok();
    }

    [HttpGet]
    public IEnumerable<MeteorologicalDataEntity> GetAll([FromQuery] int skip = 0)
    {
        return _context.MeteologicalData.Skip(0).Take(10);
    }
}
