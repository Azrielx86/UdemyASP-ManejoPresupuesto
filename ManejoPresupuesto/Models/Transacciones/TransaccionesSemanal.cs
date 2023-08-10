namespace ManejoPresupuesto.Models.Transacciones;

public class TransaccionesSemanal
{
    public decimal Egresos { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Monto { get; set; }
    public int Semana { get; set; }
    public TipoOperacion TipoOperacionId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}