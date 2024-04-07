﻿using Ardalis.SharedKernel;

namespace KuCloud.Core.ContributorAggregate.Events;

/// <summary>
///     A domain event that is dispatched whenever a contributor is deleted.
///     The DeleteContributorService is used to dispatch this event.
/// </summary>
public sealed class ContributorDeletedEvent(int contributorId) : DomainEventBase
{
    public int ContributorId { get; init; } = contributorId;
}
