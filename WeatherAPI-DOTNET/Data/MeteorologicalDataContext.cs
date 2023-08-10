using Microsoft.EntityFrameworkCore;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Context;

public class MeteorologicalDataContext: DbContext
{
    public MeteorologicalDataContext(DbContextOptions<MeteorologicalDataContext> opts): base(opts) { 
    }
    public DbSet<MeteorologicalDataEntity> MeteologicalData { get; set; }
}
