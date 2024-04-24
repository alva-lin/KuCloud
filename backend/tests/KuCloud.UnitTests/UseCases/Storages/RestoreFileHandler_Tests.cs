using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class RestoreFileHandler_Tests : BasicTest
{
    private readonly RestoreFileHandler _handler;

    public RestoreFileHandler_Tests()
    {
        _handler = new(
            Substitute.For<ILogger<RestoreFileHandler>>(),
            new NoOpMediator()
        );
    }

    public static RestoreFileCommand CreateCommand(long[] fileIds, long folderId)
    {
        return new Faker<RestoreFileCommand>()
                .CustomInstantiator(f => new(
                    fileIds, folderId
                ))
                .Generate()
            ;
    }

    // this command just call MoveFileCommand(includeDeleted: true)
    [Fact]
    public async Task File_Restore_Success()
    {
        var mockFolder = Folder_Tests.CreateFolder();
        var mockFiles = new List<FileNode>
        {
            FileNode_Tests.CreateFile(mockFolder),
            FileNode_Tests.CreateFile(mockFolder),
        };
        mockFiles.ForEach(e => e.AuditInfo.SetDeleteInfo());

        var command = CreateCommand(mockFiles.Select(e => e.Id).ToArray(), mockFolder.Id);

        await _handler.Handle(command, default);
    }
}