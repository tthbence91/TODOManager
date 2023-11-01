using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TodoManager;

/// <summary>
/// A filter attribute for validating the model state of an action before execution.
/// </summary>
public class ValidationFilterAttribute : IActionFilter
{
    /// <summary>
    /// Executed before the action method, validating the model state.
    /// If the model state is invalid, it returns a 422 Unprocessable Entity Object result with the model state errors.
    /// </summary>
    /// <param name="context">The context of the action being executed.</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }

    /// <summary>
    /// Executed after the action method.
    /// </summary>
    /// <param name="context">The context of the action that was executed.</param>
    public void OnActionExecuted(ActionExecutedContext context) { }
}