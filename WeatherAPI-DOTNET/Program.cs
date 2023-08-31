using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Repository;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Service;
using WeatherAPI_DOTNET.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default"); 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<MeteorologicalDataContext>(options =>
{

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IMeteorologicalDataRepository,MeteorologicalDataRepository>();
builder.Services.AddScoped<IMeteorologicalDataService, MeteorologicalDataService>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetco re/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DB Camp - WeatherAPI by Dante",
        Description = "An ASP.NET Core Web API for manitoring the weather forecast",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Developer GitHub",
            Url = new Uri("https://github.com/jovicarvalho")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
