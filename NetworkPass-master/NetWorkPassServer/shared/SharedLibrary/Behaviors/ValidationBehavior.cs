using FluentValidation;
using FluentValidation.Results;
using TS.MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharedLibrary.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
           var errors=failures
                .GroupBy(x=>x.PropertyName)
                .ToDictionary(
               g=>g.Key,
               g=>g.Select(x=>x.ErrorMessage)
               .ToArray());

            throw new ValidationException(failures);
        }
        
        return await next();
    }
}