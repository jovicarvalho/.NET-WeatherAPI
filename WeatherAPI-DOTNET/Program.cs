using Microsoft.EntityFrameworkCore;
using WeatherAPI_DOTNET.Context;
using WeatherAPI_DOTNET.Service;
using WeatherAPI_DOTNET.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default"); 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddDbContext<MeteorologicalDataContext>(options =>
{

    options.UseNpgsql(connectionString);
});


builder.Services.AddScoped<IMeteorologicalDataService,MeteorologicalDataService>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetco re/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
