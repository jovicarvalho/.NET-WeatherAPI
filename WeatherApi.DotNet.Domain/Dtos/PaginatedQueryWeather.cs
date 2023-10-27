using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI_DOTNET.Models;

namespace WeatherApi.DotNet.Domain.Dtos
{
    public class PaginatedQueryWeather
    {
        public IEnumerable<MeteorologicalDataEntity>? weathers { get; set; }

        public int offset { get; set; }

        public int pageSize { get; set; }

        public int pageNumber { get; set; }

        public int totalWeathers { get; set; }

        public int totalPages { get; set; }

    }
}
