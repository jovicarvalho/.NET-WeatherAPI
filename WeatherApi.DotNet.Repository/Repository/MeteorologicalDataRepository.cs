using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service.Interfaces;

namespace WeatherAPI_DOTNET.Data.Repository
{
    public class MeteorologicalDataRepository : IMeteorologicalDataRepository
    {
        private MeteorologicalDataContext _context;

        public MeteorologicalDataRepository(MeteorologicalDataContext context)
        {
            _context = context;
        }
        public void Add(MeteorologicalDataEntity metData)
        {
            _context.MeteorologicalData.Add(metData);
            _context.SaveChanges();
        }

        public IEnumerable<MeteorologicalDataEntity> GetAll(int skip)
        {
            return _context.MeteorologicalData.Skip(skip).Take(10);
        }

        public MeteorologicalDataEntity? FindByID(Guid id)
        {
            MeteorologicalDataEntity? metData = _context.MeteorologicalData.FirstOrDefault(metData => metData.Id == id);
            return metData;
        }

        public IEnumerable<MeteorologicalDataEntity> FindByCity(string cityName)
        {
            IEnumerable<MeteorologicalDataEntity> metData = _context.MeteorologicalData
                .Where(metDataList => metDataList.City == cityName)
                .OrderByDescending(metData => metData.WeatherDate)
                .Take(7)
                .ToList();
            ;
            return metData;
        }
        public MeteorologicalDataEntity FindBySpecificDateAndCity(string cityName, DateTime date)
        {
            var metData = _context.MeteorologicalData
                .FirstOrDefault(
               metData =>
               (metData.City == cityName) &&
               (metData.WeatherDate.Day == date.Day) &&
               (metData.WeatherDate.Month == date.Month) &&
               (metData.WeatherDate.Year == date.Year)
               );
            return metData;
        }

        public MeteorologicalDataEntity DeleteById(Guid id)
        {
            MeteorologicalDataEntity metData = FindByID(id);
            _context.Remove(metData);
            _context.SaveChanges();
            return metData;
        }
        public void EditMeteorologicalData()
        {
            _context.SaveChanges();
        }


    }
}
