using AutoMapper;
using WeatherAPI_DOTNET.Data.Dtos;
using WeatherAPI_DOTNET.Models;

namespace WeatherAPI_DOTNET.Profiles;

public class MeteorologicalDataProfile : Profile
{

    public MeteorologicalDataProfile()
    {
        CreateMap<CreateMetDataDto,MeteorologicalDataEntity>();
        CreateMap<UpdateMetDataDto, MeteorologicalDataEntity>();
        CreateMap<MeteorologicalDataEntity, UpdateMetDataDto>();
        CreateMap<MeteorologicalDataEntity,ReadMetDataDto>();
    }
}
