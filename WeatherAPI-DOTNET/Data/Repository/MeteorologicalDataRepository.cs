using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Data.Repository
{
    public class MeteorologicalDataRepository : IMeteorologicalDataRepository
    {
        private MeteorologicalDataContext _context;

        public MeteorologicalDataRepository(MeteorologicalDataContext context, IMapper mapper)
        {
            _context = context;
        }
        public void Add(MeteorologicalDataEntity metData)
        {
                _context.MeteorologicalData.Add(metData);
                _context.SaveChanges();
        }

        public MeteorologicalDataEntity FindByID(int id)
        {
            MeteorologicalDataEntity? metData = _context.MeteorologicalData.FirstOrDefault(metData => metData.Id == id);
            return metData;
        }

    }
}
