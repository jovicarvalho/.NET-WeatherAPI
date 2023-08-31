using Microsoft.EntityFrameworkCore;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Context;

public class MeteorologicalDataContext: DbContext
{
    public MeteorologicalDataContext(DbContextOptions<MeteorologicalDataContext> opts): base(opts) { 
    }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeteorologicalDataEntity>()
                .Property(e => e.WeatherDate)
                .HasColumnType("date");

            modelBuilder.Entity<MeteorologicalDataEntity>()
            .Property(e => e.Id)
            .HasConversion(id => id,
                id => id == Guid.Empty ? Guid.NewGuid() : id); 
        }

    public DbSet<MeteorologicalDataEntity> MeteorologicalData { get; set; }
}
