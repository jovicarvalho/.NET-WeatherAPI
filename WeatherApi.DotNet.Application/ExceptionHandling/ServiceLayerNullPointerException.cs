using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.DotNet.Application.ExceptionHandling
{
    public class ServiceLayerNullPointerException: NullReferenceException
    {
        public ServiceLayerNullPointerException() { }
        public ServiceLayerNullPointerException(string message) : base(message) { }
        public ServiceLayerNullPointerException(string message, Exception innerException) : base(message, innerException) { }
      

    }
}
