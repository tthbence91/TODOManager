using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Routing;
using TodoManager;

namespace TodoManagerTests;

public class ValidationFilterTests
{
    [Fact]
    public void OnActionExecuting_ValidModelState_ShouldNotChangeResult()
    {
        // Arrange
        var actionContext = new ActionExecutingContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            },
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            new Mock<Controller>().Object
        );

        var filter = new ValidationFilterAttribute();

        // Act
        filter.OnActionExecuting(actionContext);

        // Assert
        actionContext.Result.Should().BeNull();
    }

    [Fact]
    public void OnActionExecuting_InvalidModelState_ShouldReturnUnprocessableEntityResult()
    {
        // Arrange
        var actionContext = new ActionExecutingContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            },
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            new Mock<Controller>().Object
        );

        actionContext.ModelState.AddModelError("PropertyName", "Error Message");

        var filter = new ValidationFilterAttribute();

        // Act
        filter.OnActionExecuting(actionContext);

        // Assert
        actionContext.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var result = actionContext.Result as UnprocessableEntityObjectResult;
        result?.Value.Should().BeOfType<SerializableError>();
        var errors = result?.Value as SerializableError;

        errors.Should().NotBeNull();
        errors.Should().ContainKey("PropertyName");
    }
}