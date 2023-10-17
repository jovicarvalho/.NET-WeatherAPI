using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WeatherApi.DotNet.Domain.Entity;
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

        public async Task<PaginatedQueryWeather> GetPaginatedDataOfAllWeathers(int skip)
        {
            var pageSize = 10;
            var query = _context.MeteorologicalData.OrderByDescending(metData => metData.WeatherDate);

            int totalCountWeathers = await query.CountAsync();
            var weathers = await query.Skip(skip).Take(pageSize).ToListAsync();


            int totalPages = (int)Math.Ceiling((double)totalCountWeathers / pageSize);

            var pagedData = new PaginatedQueryWeather
            {
                pageSize = pageSize,
                totalPages = totalPages,
                totalWeathers = totalCountWeathers,
                weathers = weathers,
                offset = 0,
                pageNumber = skip
            };

            return pagedData;
        }

        //public async Task<PaginatedQueryWeather> GetPaginatedDataOfAllWeathers(int skip)
        //{
        //    var pageSize = 10;
        //    var query = _context.MeteorologicalData.OrderByDescending(metData => metData.WeatherDate);

        //    var totalWeathers = query.CountAsync();
        //    var data = query.Skip(skip).Take(pageSize).ToListAsync();

        //    await Task.WhenAll(totalWeathers, data);

        //    int totalPages = (int)Math.Ceiling((double)totalWeathers.Result / pageSize);

        //    var pagedData = new PaginatedQueryWeather
        //    {
        //        pageSize = pageSize,
        //        totalPages = totalPages,
        //        totalWeathers = totalWeathers.Result,
        //        weathers = data.Result,
        //        offset = 0,
        //        pageNumber = skip
        //    };

        //    return pagedData;
        //}

        public IEnumerable<MeteorologicalDataEntity> FindWeekInCity(string cityName) {
            var weekInCity = _context.MeteorologicalData
                .Where(metData=> metData.City == cityName)
                .OrderByDescending(metData => metData.WeatherDate)
                .Take(7)
                ;
            return weekInCity;
        }

        public MeteorologicalDataEntity? FindByID(Guid id)
        {
            MeteorologicalDataEntity? metData = _context.MeteorologicalData.FirstOrDefault(metData => metData.Id == id);
            return metData;
        }

        public async Task<PaginatedQueryWeather> GetPaginatedDataByCity(string cityName, int skip)
        {
            var pageSize = 10;
            var query = _context.MeteorologicalData
                .Where(metDataList => metDataList.City == cityName)
                .OrderByDescending(metData => metData.WeatherDate);

            var totalWeathersByCity = await query.CountAsync();
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalWeathersByCity / pageSize);

            var pagedData = new PaginatedQueryWeather
            {
                pageSize = pageSize,
                totalPages = totalPages,
                totalWeathers = totalWeathersByCity,
                weathers = data,
                offset = 0,
                pageNumber = (skip / 10)
            };

            return pagedData;
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
