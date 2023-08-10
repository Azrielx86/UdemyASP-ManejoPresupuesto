using ManejoPresupuesto.Models;
using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Services;

public class ReportService : IReportService
{
    private readonly HttpContext httpContext;
    private readonly IRepositoryTransactions repositoryTransactions;

    public ReportService(IRepositoryTransactions repositoryTransactions,
        IHttpContextAccessor httpContextAccessor)
    {
        this.repositoryTransactions = repositoryTransactions;
        httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<ReporteTransacciones> GetDetailedTransactions(int uid, int month, int year, dynamic ViewBag)
    {
        (var fechaInicio, var fechaFin) = GenerateDates(year, month);
        var param = new TransaccionesPorUsuario()
        {
            FechaFin = fechaFin,
            FechaInicio = fechaInicio,
            UsuarioId = uid
        };

        var transacciones = await repositoryTransactions.GetByUser(param);

        var modelo = new ReporteTransacciones();

        var tbydate = transacciones
                        .OrderByDescending(t => t.FechaTransaccion)
                        .GroupBy(t => t.FechaTransaccion)
                        .Select(grupo => new ReporteTransacciones.TransaccionesPorFecha()
                        {
                            FechaTransaccion = grupo.Key,
                            Transacciones = grupo.AsEnumerable()
                        });

        modelo.Transacciones = tbydate;
        modelo.FechaInicio = fechaInicio;
        modelo.FechaFin = fechaFin;
        //ViewBag.pastMonth = fechaInicio.AddMonths(-1).Month;
        //ViewBag.pastYear = fechaInicio.AddMonths(-1).Year;
        //ViewBag.nextMonth = fechaInicio.AddMonths(1).Month;
        //ViewBag.nextYear = fechaInicio.AddMonths(1).Year;
        //ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        AssignViewBagValues(ViewBag, fechaInicio);
        return modelo;
    }

    public async Task<ReporteTransacciones> GetTransactionReportByUser(int uid, int cuentaId, int month, int year, dynamic ViewBag)
    {
        (var fechaInicio, var fechaFin) = GenerateDates(year, month);
        var param = new TransaccionesPorUsuario()
        {
            FechaFin = fechaFin,
            FechaInicio = fechaInicio,
            UsuarioId = uid
        };

        var transacciones = await repositoryTransactions.GetByUser(param);

        var modelo = new ReporteTransacciones();

        var tbydate = transacciones
                        .OrderByDescending(t => t.FechaTransaccion)
                        .GroupBy(t => t.FechaTransaccion)
                        .Select(grupo => new ReporteTransacciones.TransaccionesPorFecha()
                        {
                            FechaTransaccion = grupo.Key,
                            Transacciones = grupo.AsEnumerable()
                        });

        modelo.Transacciones = tbydate;
        modelo.FechaInicio = fechaInicio;
        modelo.FechaFin = fechaFin;
        AssignViewBagValues(ViewBag, fechaInicio);
        //ViewBag.pastMonth = fechaInicio.AddMonths(-1).Month;
        //ViewBag.pastYear = fechaInicio.AddMonths(-1).Year;
        //ViewBag.nextMonth = fechaInicio.AddMonths(1).Month;
        //ViewBag.nextYear = fechaInicio.AddMonths(1).Year;
        //ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        return modelo;
    }

    public async Task<IEnumerable<TransaccionesSemanal>> GetWeeklyReport(int uid, int month, int year, dynamic ViewBag)
    {
        (var fechaInicio, var fechaFin) = GenerateDates(year, month);
        var param = new TransaccionesPorUsuario()
        {
            FechaFin = fechaFin,
            FechaInicio = fechaInicio,
            UsuarioId = uid
        };

        AssignViewBagValues(ViewBag, fechaInicio);
        var modelo = await repositoryTransactions.GetWeekly(param);
        return modelo;
    }

    private void AssignViewBagValues(dynamic ViewBag, DateTime fechaInicio)
    {
        ViewBag.pastMonth = fechaInicio.AddMonths(-1).Month;
        ViewBag.pastYear = fechaInicio.AddMonths(-1).Year;
        ViewBag.nextMonth = fechaInicio.AddMonths(1).Month;
        ViewBag.nextYear = fechaInicio.AddMonths(1).Year;
        ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
    }

    private (DateTime fechaInicio, DateTime fechaFin) GenerateDates(int year, int month)
    {
        DateTime fechaInicio;
        DateTime fechaFin;

        if (month <= 0 || month > 12 || year <= 1900)
        {
            var today = DateTime.Today;
            fechaInicio = new DateTime(today.Year, today.Month, 1);
        }
        else
        {
            fechaInicio = new DateTime(year, month, 1);
        }

        fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

        return (fechaInicio, fechaFin);
    }
}