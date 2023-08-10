using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Models;

#nullable disable

public class ReporteSemanalViewModel
{
    public IEnumerable<TransaccionesSemanal> Transacciones { get; set; }
    public decimal Ingresos => Transacciones.Sum(s => s.Ingresos);
    public decimal Gastos => Transacciones.Sum(s => s.Egresos);
    public decimal Total => Ingresos - Gastos;
    public DateTime FechaReferencia { get; set; }
}