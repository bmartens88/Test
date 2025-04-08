using Microsoft.AspNetCore.Http;
using Test.Api.Common.Domain;
using Test.Api.Common.Domain.Errors;

namespace Test.Api.Common.Presentation.Results;

public static class ApiResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException();

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));

        static string GetTitle(Error error)
        {
            return error.Type.Name switch
            {
                nameof(ErrorType.Validation) => error.Code,
                nameof(ErrorType.Problem) => error.Code,
                nameof(ErrorType.NotFound) => error.Code,
                nameof(ErrorType.Conflict) => error.Code,
                _ => "Server failure"
            };
        }

        static string GetDetail(Error error)
        {
            return error.Type.Name switch
            {
                nameof(ErrorType.Validation) => error.Description,
                nameof(ErrorType.Problem) => error.Description,
                nameof(ErrorType.NotFound) => error.Description,
                nameof(ErrorType.Conflict) => error.Description,
                _ => "An unexpected error occurred"
            };
        }

        static string GetType(ErrorType errorType)
        {
            return errorType.Name switch
            {
                nameof(ErrorType.Validation) => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                nameof(ErrorType.Problem) => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                nameof(ErrorType.NotFound) => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                nameof(ErrorType.Conflict) => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
        }

        static int GetStatusCode(ErrorType errorType)
        {
            return errorType.Name switch
            {
                nameof(ErrorType.Validation) => StatusCodes.Status400BadRequest,
                nameof(ErrorType.Problem) => StatusCodes.Status400BadRequest,
                nameof(ErrorType.NotFound) => StatusCodes.Status404NotFound,
                nameof(ErrorType.Conflict) => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        static Dictionary<string, object?>? GetErrors(Result result)
        {
            if (result.Error is not ValidationError validationError)
                return null;

            return new Dictionary<string, object?>
            {
                { "errors", validationError.Errors }
            };
        }
    }
}