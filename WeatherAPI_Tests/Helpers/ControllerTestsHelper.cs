using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.DotNet.Tests.Helpers;

internal class ControllerTestsHelper
{
    public void MockModelState<TModel, TController>(TModel model, TController controller) where TController : ControllerBase
    {
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        foreach (var validationResult in validationResults)
        {
            controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
        }
    }
}
