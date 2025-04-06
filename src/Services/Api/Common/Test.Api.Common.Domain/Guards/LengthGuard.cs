using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace Test.Api.Common.Domain.Guards;

public static class LengthGuard
{
    public static string Length(this IGuardClause _,
        string input,
        int maxLength,
        [CallerArgumentExpression(nameof(input))]
        string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        if (input.Length > maxLength)
            throw new ArgumentException($"Should not exceed length of '{maxLength}' characters.", paramName);
        return input;
    }
}