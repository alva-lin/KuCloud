using Ardalis.Result;
using Ardalis.SharedKernel;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;
using Microsoft.Extensions.Logging;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class RestoreFolderHandler_Tests : BasicTest
{
    private readonly RestoreFolderHandler _handler;

    public RestoreFolderHandler_Tests()
    {
        _handler = new RestoreFolderHandler(Substitute.For<ILogger<RestoreFolderHandler>>(), new NoOpMediator());
    }

    public static RestoreFolderCommand CreateCommand(long id,long folderId)
    {
        return new Faker<RestoreFolderCommand>()
                .CustomInstantiator(f => new(
                    id,
                    folderId
                ))
                .Generate()
            ;
    }

    // this command just call MoveFolderCommand(includeDeleted: true)
    [Fact]
    public async Task Folder_Restore_Success()
    {
        var mockParent = new Folder("Parent", null);
        var mockFolder = new Folder("Folder", null);
        mockFolder.AuditInfo.SetDeleteInfo();
        var command = CreateCommand(mockFolder.Id, mockParent.Id);

        await _handler.Handle(command, default);
    }
}
