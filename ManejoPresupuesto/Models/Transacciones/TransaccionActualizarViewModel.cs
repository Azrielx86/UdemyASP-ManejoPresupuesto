namespace ManejoPresupuesto.Models.Transacciones
{
    public class TransaccionActualizarViewModel : TransaccionCrearViewModel
    {
        public decimal MontoAnterior { get; set; }
        public int CuentaAnteriorId { get; set; }
        public string? UrlRetorno { get; set; }
    }
}