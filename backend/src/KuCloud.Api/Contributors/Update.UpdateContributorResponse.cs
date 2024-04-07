namespace KuCloud.Api.Contributors;

public class UpdateContributorResponse(ContributorRecord contributor)
{
    public ContributorRecord Contributor { get; set; } = contributor;
}