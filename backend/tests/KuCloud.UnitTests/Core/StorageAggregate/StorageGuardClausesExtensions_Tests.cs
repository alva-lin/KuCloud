using Ardalis.GuardClauses;
using KuCloud.Core.StorageAggregate;

namespace KuCloud.UnitTests.Core.StorageAggregate;

public class StorageGuardClausesExtensions_Tests
{
    [Fact]
    public void CheckInvalidPath_WithInvalidPath_ShouldThrowArgumentException()
    {
        // Arrange
        var inputs = new[]
        {
            string.Empty,
            "  ",
            "invalid/path",
            "invalid\\path"
        };

        foreach (var input in inputs)
        {
            // Act
            Action act = () => Guard.Against.CheckInvalidPath(input);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        Assert.Throws<ArgumentNullException>(() => Guard.Against.CheckInvalidPath(null));

    }

    [Fact]
    public void CheckInvalidPath_WithValidPath_ShouldNotThrowArgumentException()
    {
        // Arrange
        var input = "validpath";

        // Act
        Action act = () => Guard.Against.CheckInvalidPath(input);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }
}
