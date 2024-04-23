namespace KuCloud.UseCases.Storages;

public record RenameFileCommand(long FileId, string NewName) : ICommand<Result>;
