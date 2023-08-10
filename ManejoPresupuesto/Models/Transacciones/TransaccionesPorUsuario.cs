namespace ManejoPresupuesto.Models.Transacciones;

public class TransaccionesPorUsuario
{
    public int UsuarioId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}