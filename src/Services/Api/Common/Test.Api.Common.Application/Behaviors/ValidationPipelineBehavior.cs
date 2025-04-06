using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Test.Api.Common.Application.Messaging;
using Test.Api.Common.Domain;
using Test.Api.Common.Domain.Errors;

namespace Test.Api.Common.Application.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationFailures = await ValidateAsync(request);

        if (validationFailures.Length is 0)
            return await next(cancellationToken);

        if (typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.ValidationFailure));

            if (failureMethod is not null)
                return (TResponse)failureMethod.Invoke(null, [CreateValidationError(validationFailures)])!;
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(CreateValidationError(validationFailures));
        }

        throw new ValidationException(validationFailures);
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!_validators.Any())
            return [];

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context)));

        var validationFailures = validationResults
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures)
    {
        return new ValidationError(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage))
            .ToArray());
    }
}