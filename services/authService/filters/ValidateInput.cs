using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace authService.filters;

/// <summary>
/// Action filter that validates model state and returns validation errors automatically.
/// Implements IActionFilter to intercept action execution and check for validation errors.
/// </summary>
public class ValidateInput : IActionFilter
{
    /// <summary>
    /// Called after the action method executes. Currently not implemented.
    /// </summary>
    /// <param name="context">The action executed context</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }

    /// <summary>
    /// Called before the action method executes. Validates the model state and returns
    /// a BadRequest response with validation errors if the model state is invalid.
    /// </summary>
    /// <param name="context">The action executing context containing the model state</param>
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