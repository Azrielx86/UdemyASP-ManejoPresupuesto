namespace ManejoPresupuesto.Models;

public class TransaccionesPorFecha
{
    public int CuentaId { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaInicio { get; set; }
    public int UsuarioId { get; set; }
}