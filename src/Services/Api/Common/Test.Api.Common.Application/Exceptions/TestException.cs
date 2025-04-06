using Test.Api.Common.Domain.Errors;

namespace Test.Api.Common.Application.Exceptions;

public sealed class TestException(string requestName, Error? error = null, Exception? innerException = null)
    : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;

    public Error? Error { get; } = error;
}