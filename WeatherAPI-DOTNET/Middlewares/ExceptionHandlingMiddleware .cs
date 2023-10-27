using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WeatherApi.DotNet.Application.ExceptionHandling;
namespace WeatherAPI_DOTNET.Middlewares
{
    public class ExceptionHandlingMiddleware
    {   
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if(ex is ServiceLayerNullPointerException) {
                    httpContext.Response.StatusCode = 404;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync($"{{ \"error\": \"{ex.Message}\" }}");
                }
            }
        }
    }
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
