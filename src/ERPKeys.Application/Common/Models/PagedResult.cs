using ERPKeys.Domain.Modules.ProductManagement;

namespace ERPKeys.Application.Common.Models;

//Generic for pagination
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages) where T : class;
