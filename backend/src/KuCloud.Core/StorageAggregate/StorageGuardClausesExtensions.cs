using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace KuCloud.Core.StorageAggregate;

public static class StorageGuardClausesExtensions
{
    public static string CheckInvalidPath(
        this IGuardClause guardClause,
        [NotNull, ValidatedNotNull] string? input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null
    )
    {
        guardClause.NullOrWhiteSpace(input, parameterName, message);

        if (input.Contains('/') || input.Contains('\\'))
        {
            throw new ArgumentException(message ?? "Path contains invalid characters", parameterName);
        }

        return input;
    }
}
