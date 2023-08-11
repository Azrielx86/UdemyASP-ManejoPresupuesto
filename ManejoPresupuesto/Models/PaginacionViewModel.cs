namespace ManejoPresupuesto.Models;

public class PaginacionViewModel
{
    public int Page { get; set; } = 1;
    private int recordsPerPage = 10;
    private readonly int maxRecPerPage = 50;

    public int RecordsPerPage
    {
        get => recordsPerPage;
        set => recordsPerPage = (value > maxRecPerPage) ? maxRecPerPage : value;
    }

    public int RecordsIgnore => recordsPerPage * (Page - 1);
}