namespace ManejoPresupuesto.Models;

public class PageResponse
{
    public int Page { get; set; } = 1;
    public int RecordsPerPage { get; set; } = 10;
    public int TotalRecordCount { get; set; }
    public int TotalPageCount => (int)Math.Ceiling((double)TotalRecordCount / RecordsPerPage);
    public string? BaseUrl { get; set; }
}

public class PageResponse<T> : PageResponse
{
    public IEnumerable<T>? Items { get; set; }
}