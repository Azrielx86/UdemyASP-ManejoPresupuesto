namespace ManejoPresupuesto.Models.Transacciones;

public class TransaccionesMensual
{
    public int Month { get; set; }
    public DateTime FechaReferencia { get; set; }
    public decimal Monto { get; set; }
    public TipoOperacion TipoOperacionId { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Gastos { get; set; }
}