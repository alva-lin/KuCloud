using System.Diagnostics;
using Ardalis.GuardClauses;
using KuCloud.Core.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

namespace KuCloud.Infrastructure.Behaviors;

public class MyLoggingBehavior<TRequest, TResponse>(ILogger<Mediator> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var ignore = propertyInfo.GetCustomAttributes(typeof(LogIgnoreAttribute), false).Any();
                var obj = ignore ? "*log ignore*" : propertyInfo?.GetValue(request, null);
                logger.LogInformation("Property {Property} : {@Value}", propertyInfo?.Name, obj);
            }
        }

        var sw        = Stopwatch.StartNew();
        var response1 = await next();
        logger.LogInformation("Handled {RequestName} with {Response} in {Ms} ms", typeof(TRequest).Name,
            response1, sw.ElapsedMilliseconds);
        sw.Stop();
        return response1;
    }
}
