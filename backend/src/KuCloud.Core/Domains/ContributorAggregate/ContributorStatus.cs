using Ardalis.SmartEnum;

namespace KuCloud.Core.Domains.ContributorAggregate;

public sealed class ContributorStatus(string name, int value) : SmartEnum<ContributorStatus>(name, value)
{
    // ReSharper disable once UnusedMember.Local
    public ContributorStatus() : this("Unknown", 0) { }

    public static readonly ContributorStatus CoreTeam = new(nameof(CoreTeam), 1);
    public static readonly ContributorStatus Community = new(nameof(Community), 2);
    public static readonly ContributorStatus NotSet = new(nameof(NotSet), 3);
}
