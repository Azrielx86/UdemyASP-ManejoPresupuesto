namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizarViewModel : TransaccionCrearViewModel
    {
        public decimal MontoAnterior { get; set; }
        public int CuentaAnteriorId { get; set; }
    }
}