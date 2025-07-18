using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace authService.filters;

public class ValidateInput : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Any())
                .SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage))
                .ToList();

            context.Result = new BadRequestObjectResult(new
            {
                Message = "Validation errors occurred.",
                Errors = errors
            });
        }
    }
}