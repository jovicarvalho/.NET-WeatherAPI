using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WeatherApi.DotNet.Domain.Dtos;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Models;
using WeatherAPI_DOTNET.Service.Interfaces;

namespace WeatherAPI_DOTNET.Data.Repository
{
    public class MeteorologicalDataRepository : IMeteorologicalDataRepository
    {
        private readonly MeteorologicalDataContext _context;

        public MeteorologicalDataRepository(MeteorologicalDataContext context)
        {
            _context = context;
        }
        public async Task Add(MeteorologicalDataEntity metData)
        {
            await _context.MeteorologicalData.AddAsync(metData);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedQueryWeather?> GetPaginatedDataOfAllWeathers(int skip)
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


        public async Task<IEnumerable<MeteorologicalDataEntity>> FindWeekInCity(string cityName) {
            var weekInCity = await _context.MeteorologicalData
                .Where(metData => metData.City == cityName)
                .OrderByDescending(metData => metData.WeatherDate)
                .Take(7)
                .ToListAsync();

            return weekInCity;
        }

        public async Task<MeteorologicalDataEntity?> FindByID(Guid id)
        {
            var metData = await _context.MeteorologicalData.FirstOrDefaultAsync(metData => metData.Id == id);
            return metData;
        }

        public async Task<PaginatedQueryWeather?> GetPaginatedDataByCity(string cityName, int skip)
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


        public async Task<MeteorologicalDataEntity?> FindBySpecificDateAndCity(string cityName, DateTime date)
        {
            var metData = await _context.MeteorologicalData
                .FirstOrDefaultAsync(
               metData =>
               (metData.City == cityName) &&
               (metData.WeatherDate.Day == date.Day) &&
               (metData.WeatherDate.Month == date.Month) &&
               (metData.WeatherDate.Year == date.Year)
               );
            return metData;
        }

        public async Task<MeteorologicalDataEntity?> DeleteById(MeteorologicalDataEntity weather)
        {
            _context.Remove(weather);
            await _context.SaveChangesAsync();
            return weather;
        }
        public async Task EditMeteorologicalData()
        {
            await _context.SaveChangesAsync();
        }


    }
}
