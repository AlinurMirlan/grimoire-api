
using FluentValidation;

namespace Grimoire.Api.Infrastructure.FIlters;

public class ValidationFilter<TModel> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<TModel>>();
        if (validator is null)
        {
            return await next(context);
        }

        var model = context.Arguments.OfType<TModel>().FirstOrDefault();
        if (model is null)
        {
            return Results.Problem("Could not find type to validate.");
        }

        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}
