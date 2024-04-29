using Ardalis.Result;

namespace KuCloud.SharedKernel;

public record PaginatedList<T>(IEnumerable<T> Data, long Count)
{
    public static implicit operator PaginatedList<T>((IEnumerable<T> Data, long Count) tuple) =>
        new(tuple.Data, tuple.Count);

    public static explicit operator Result<PaginatedList<T>>(PaginatedList<T> paginatedList) =>
        Result<PaginatedList<T>>.Success(paginatedList);
}
