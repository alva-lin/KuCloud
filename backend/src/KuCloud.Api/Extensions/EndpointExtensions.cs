using System.Net;
using Ardalis.GuardClauses;
using FluentValidation.Results;

namespace KuCloud.Api.Extensions;

public static class EndpointExtensions
{
    private static ValidationFailure AsValidationFailure(this ValidationError validationError)
    {
        return new ValidationFailure(
            validationError.Identifier,
            validationError.ErrorMessage
        )
        {
            Severity = FromValidationSeverity(validationError.Severity)
        };

        Severity FromValidationSeverity(ValidationSeverity severity)
        {
            return severity switch
            {
                ValidationSeverity.Error => Severity.Error,
                ValidationSeverity.Warning => Severity.Warning,
                ValidationSeverity.Info => Severity.Info,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), "Unexpected ValidationSeverity")
            };
        }
    }

    private static void AddErrors<TRequest, TResponse, T>(this Endpoint<TRequest, TResponse> endpoint, Result<T> result)
        where TRequest : notnull
    {
        if (result.IsSuccess)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            // Error Pattern => PropertyName, PropertyValue, ErrorCode, Severity, ErrorMessage
            // 如果 error 符合 Error Pattern，則將其轉換為 new ValidationFailure(propertyName, errorMessage)
            // 否则直接 new ValidationFailure(error)
            var errorParts = error.Split(',');
            if (errorParts.Length == 3)
            {
                var propertyName = errorParts[0];
                var errorMessage = errorParts[1];
                var severity     = errorParts[2];

                endpoint.AddError(new ValidationFailure(propertyName, errorMessage)
                {
                    Severity = severity switch
                    {
                        "Error"   => Severity.Error,
                        "Warning" => Severity.Warning,
                        "Info"    => Severity.Info,
                        _         => throw new ArgumentOutOfRangeException(nameof(severity), "Unexpected Severity")
                    }
                });
            }
            else
            {
                endpoint.AddError(error);
            }
        }

        foreach (var validationError in result.ValidationErrors)
        {
            endpoint.AddError(validationError.AsValidationFailure());
        }
    }

    public static void CheckResult<TRequest, TResponse, T>(this Endpoint<TRequest, TResponse> endpoint,
       [ValidatedNotNull] Result<T> result) where TRequest : notnull
    {
        endpoint.AddErrors(result);

        HttpStatusCode? code = result.Status switch
        {
            ResultStatus.Ok            => HttpStatusCode.OK,
            ResultStatus.Error         => HttpStatusCode.InternalServerError,
            ResultStatus.Forbidden     => HttpStatusCode.Forbidden,
            ResultStatus.Unauthorized  => HttpStatusCode.Unauthorized,
            ResultStatus.Invalid       => HttpStatusCode.BadRequest,
            ResultStatus.NotFound      => HttpStatusCode.NotFound,
            ResultStatus.Conflict      => HttpStatusCode.Conflict,
            ResultStatus.CriticalError => HttpStatusCode.InternalServerError,
            ResultStatus.Unavailable   => HttpStatusCode.ServiceUnavailable,
            _                          => null
        };

        endpoint.ThrowIfAnyErrors((int?)code);
    }
}
