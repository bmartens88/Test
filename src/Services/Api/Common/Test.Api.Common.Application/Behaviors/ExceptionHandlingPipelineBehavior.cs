using MediatR;
using Microsoft.Extensions.Logging;
using Test.Api.Common.Application.Exceptions;

namespace Test.Api.Common.Application.Behaviors;

internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for '{RequestName}'", typeof(TRequest).Name);

            throw new TestException(typeof(TRequest).Name, innerException: ex);
        }
    }
}