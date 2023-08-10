using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Models;

#nullable disable

public class ReporteTransacciones
{
    public decimal BalanceDepositos => Transacciones.Sum(_ => _.BalanceDepositos);
    public decimal BalanceRetiros => Transacciones.Sum(_ => _.BalanceRetiros);
    public DateTime FechaFin { get; set; }
    public DateTime FechaInicio { get; set; }
    public decimal Total => BalanceDepositos - BalanceRetiros;
    public IEnumerable<TransaccionesPorFecha> Transacciones { get; set; }

    public class TransaccionesPorFecha
    {
        public decimal BalanceDepositos => (from t in Transacciones
                                            where t.TipoOperacionId == TipoOperacion.Ingreso
                                            select t).Sum(t => t.Monto);

        public decimal BalanceRetiros => (from t in Transacciones
                                          where t.TipoOperacionId == TipoOperacion.Gasto
                                          select t).Sum(t => t.Monto);

        public DateTime FechaTransaccion { get; set; }
        public IEnumerable<Transaccion> Transacciones { get; set; }
    }
}