﻿using Ardalis.GuardClauses;
using Ardalis.SharedKernel;

namespace KuCloud.Core.Domains.ContributorAggregate;

public class Contributor(string name) : BasicEntity<int>, IAggregateRoot
{
    // Example of validating primary constructor inputs
    // See: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/primary-constructors#initialize-base-class
    public string Name { get; private set; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public ContributorStatus Status { get; private set; } = ContributorStatus.NotSet;
    public PhoneNumber? PhoneNumber { get; private set; }

    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = new PhoneNumber(string.Empty, phoneNumber, string.Empty);
    }

    public void UpdateName(string newName)
    {
        Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    }
}

public class PhoneNumber(
    string countryCode,
    string number,
    string? extension)
    : ValueObject
{
    public string CountryCode { get; init; } = countryCode;
    public string Number { get; init; } = number;
    public string? Extension { get; init; } = extension;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
        yield return Extension ?? string.Empty;
    }
}