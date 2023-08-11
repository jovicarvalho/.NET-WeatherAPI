using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeatherAPI_DOTNET.Context;

namespace WeatherAPI_DOTNET.Service;

public class MeteorologicalDataService
{
    private MeteorologicalDataContext _context;
    private IMapper _mapper;
    public MeteorologicalDataService(MeteorologicalDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
}
