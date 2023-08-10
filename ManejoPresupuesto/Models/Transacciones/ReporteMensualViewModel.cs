namespace ManejoPresupuesto.Models.Transacciones;

#nullable disable

public class ReporteMensualViewModel
{
    public IEnumerable<TransaccionesMensual> Transacciones { get; set; }
    public decimal Ingresos => Transacciones.Sum(_ => _.Ingresos);
    public decimal Gastos => Transacciones.Sum(_ => _.Gastos);
    public decimal Total => Ingresos - Gastos;
    public int Year { get; set; }
}