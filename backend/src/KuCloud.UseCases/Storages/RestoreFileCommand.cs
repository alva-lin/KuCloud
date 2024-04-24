namespace KuCloud.UseCases.Storages;

public record RestoreFileCommand(long[] Ids, long FolderId) : ICommand<Result>;

public sealed class RestoreFileHandler(
    ILogger<RestoreFileHandler> logger,
    IMediator mediator)
    : ICommandHandler<RestoreFileCommand, Result>
{
    public async Task<Result> Handle(RestoreFileCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RestoreFileCommand)} {request}");

        // 需要判断父亲是否存在
        var moveCommand = new MoveFileCommand(request.Ids, request.FolderId, IncludeDeleted: true);
        var result = await mediator.Send(moveCommand, ct);

        return result;
    }
}
