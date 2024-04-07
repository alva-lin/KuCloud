using Bogus;

namespace KuCloud.TestShared;

public abstract class BasicTest
{
    protected Faker Fake { get; } = new();
}
