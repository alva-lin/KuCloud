namespace KuCloud.UseCases.Storages;

public record RestoreFolderCommand(long Id, long ParentId) : ICommand<Result>;

public sealed class RestoreFolderHandler(
    ILogger<RestoreFolderHandler> logger,
    IMediator mediator)
    : ICommandHandler<RestoreFolderCommand, Result>
{
    public async Task<Result> Handle(RestoreFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RestoreFolderCommand)} {request}");

        // 需要判断父亲是否存在
        var moveCommand = new MoveFolderCommand(request.Id, request.ParentId, IncludeDeleted: true);
        var result = await mediator.Send(moveCommand, ct);

        return result;
    }
}
