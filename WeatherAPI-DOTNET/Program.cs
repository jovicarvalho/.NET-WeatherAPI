using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Data.Repository;
using WeatherAPI_DOTNET.Data.Repository.Interfaces;
using WeatherAPI_DOTNET.Middlewares;
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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WeatherFront",
        builder => builder
            .WithOrigins("http://localhost:3000", "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DB Camp - WeatherAPI by Dante",
        Description = "An ASP.NET Core Web API for manitoring the weather forecast",
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("WeatherFront");

app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();   

app.Run();

