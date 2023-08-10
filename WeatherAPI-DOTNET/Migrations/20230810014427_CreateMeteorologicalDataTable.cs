using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WeatherAPI_DOTNET.Migrations
{
    /// <inheritdoc />
    public partial class CreateMeteorologicalDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeteologicalData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    WeatherDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MorningWeather = table.Column<string>(type: "text", nullable: false),
                    NightWeather = table.Column<string>(type: "text", nullable: false),
                    MaxTemperature = table.Column<int>(type: "integer", nullable: false),
                    MinTemperature = table.Column<int>(type: "integer", nullable: false),
                    humidity = table.Column<int>(type: "integer", nullable: false),
                    Precipitation = table.Column<int>(type: "integer", nullable: false),
                    WindSpeed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeteologicalData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeteologicalData");
        }
    }
}
