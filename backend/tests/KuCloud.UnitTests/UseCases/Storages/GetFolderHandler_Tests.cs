using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UnitTests.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages;

namespace KuCloud.UnitTests.UseCases.Storages;

public sealed class GetFolderHandler_Tests : BasicTest
{
    private readonly IReadRepository<Folder> _repository;
    private readonly GetFolderHandler _handler;

    public GetFolderHandler_Tests()
    {
        _repository = Substitute.For<IReadRepository<Folder>>();
        _handler = new GetFolderHandler(_repository);
    }

    public static GetFolderQuery CreateQuery(long id)
    {
        return new Faker<GetFolderQuery>().CustomInstantiator(_ => new GetFolderQuery(id)).Generate();
    }

    [Fact]
    public async Task Handle_WhenFolderNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = CreateQuery(1);
        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>())
            .Returns((Folder?) null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repository.Received(1).SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFolderFound_ReturnsFolder()
    {
        // Arrange
        var mockFolder = Folder_Tests.CreateFolder();
        var mockDto = FolderDto.Map(mockFolder);
        var query = CreateQuery(mockFolder.Id);

        _repository.SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>()).Returns(mockFolder);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(mockDto);

        await _repository.Received(1).SingleOrDefaultAsync(Arg.Any<SingleFolderById>(), Arg.Any<CancellationToken>());
    }
}
