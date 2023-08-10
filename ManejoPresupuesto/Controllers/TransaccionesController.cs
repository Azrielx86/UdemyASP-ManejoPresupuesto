using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Models.Transacciones;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace ManejoPresupuesto.Controllers;

public class TransaccionesController : Controller
{
    private readonly IMapper mapper;
    private readonly IReportService reportService;
    private readonly IRepositoryAccounts repositoryAccounts;
    private readonly IRepositoryCategory repositoryCategory;
    private readonly IRepositoryTransactions repositoryTransactions;
    private readonly IUserService userService;

    public TransaccionesController(IUserService userService,
        IRepositoryAccounts repositoryAccounts,
        IRepositoryCategory repositoryCategory,
        IRepositoryTransactions repositoryTransactions,
        IReportService reportService,
        IMapper mapper)
    {
        this.userService = userService;
        this.repositoryAccounts = repositoryAccounts;
        this.repositoryCategory = repositoryCategory;
        this.repositoryTransactions = repositoryTransactions;
        this.reportService = reportService;
        this.mapper = mapper;
    }

    public IActionResult Calendario() => View();

    [HttpGet]
    public async Task<JsonResult> CalendarJson(DateTime start, DateTime end)
    {
        var uid = userService.GetUserId();
        var transacciones = await repositoryTransactions.GetByUser(new TransaccionesPorUsuario()
        {
            UsuarioId = uid,
            FechaInicio = start,
            FechaFin = end
        });

        var modelo = transacciones.Select(t => new EventCalendar()
        {
            Title = $"${t.Monto}",
            Start = t.FechaTransaccion.ToString("yyyy-MM-dd"),
            End = t.FechaTransaccion.ToString("yyyy-MM-dd"),
            Color = t.TipoOperacionId == TipoOperacion.Ingreso ? "#00B0FF" : "#FF1744"
        });

        return Json(modelo);
    }

    [HttpGet]
    public async Task<JsonResult> EventsByDayJson(DateTime fecha)
    {
        var uid = userService.GetUserId();
        var transacciones = await repositoryTransactions.GetByUser(new TransaccionesPorUsuario()
        {
            UsuarioId = uid,
            FechaInicio = fecha,
            FechaFin = fecha
        });
        return Json(transacciones);
    }

    public async Task<IActionResult> Create()
    {
        var userId = userService.GetUserId();
        var model = new TransaccionCrearViewModel
        {
            Cuentas = await GetAccounts(userId),
            Categorias = await GetCategories(userId, TipoOperacion.Ingreso)
        };
        return View(model);
    }

    /// <summary>
    /// Crea una transacción
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(TransaccionCrearViewModel model)
    {
        var userId = userService.GetUserId();
        if (!ModelState.IsValid)
        {
            model.Cuentas = await GetAccounts(userId);
            model.Categorias = await GetCategories(userId, TipoOperacion.Ingreso);
            return View(model);
        }

        var cuentas = await repositoryAccounts.GetById(model.CuentaId, userId);
        if (cuentas is null)
            return RedirectToAction("NoEncontrado", "Home");

        var categorias = await repositoryCategory.GetById(model.CategoriaId, userId);
        if (categorias is null)
            return RedirectToAction("NoEncontrado", "Home");

        if (model.TipoOperacionId == TipoOperacion.Gasto)
            model.Monto *= -1;

        model.UsuarioId = userId;
        await repositoryTransactions.Create(model);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, string urlRetorno = null!)
    {
        var uid = userService.GetUserId();
        var transaccion = repositoryTransactions.GetById(id, uid);
        if (transaccion is null)
            return RedirectToAction("NoEncontrado", "Home");
        await repositoryTransactions.Delete(id);
        if (string.IsNullOrEmpty(urlRetorno))
            return RedirectToAction("Index");
        else
            return LocalRedirect(urlRetorno);
    }

    public async Task<IActionResult> ExcelReporte(int month, int year)
    {
        var uid = userService.GetUserId();
        var modelo = await reportService.GetDetailedTransactions(uid, month, year, ViewBag);
        return View(modelo);
    }

    public async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
            => (await repositoryAccounts.Search(userId)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

    [HttpPost]
    public async Task<IActionResult> GetCategories([FromBody] TipoOperacion tipoOperacion)
    {
        var userId = userService.GetUserId();
        var categories = await GetCategories(userId, tipoOperacion);
        return Ok(categories);
    }

    [HttpGet]
    public async Task<FileResult> GetExcelFileMonth(int month, int year)
    {
        var uid = userService.GetUserId();
        var fechaInicio = new DateTime(year, month, 1);
        var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

        var transacciones = await repositoryTransactions.GetByUser(new TransaccionesPorUsuario()
        {
            UsuarioId = uid,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
        });

        var filename = $"Transacciones {fechaInicio:MMM yyyy} - {fechaFin:MMM yyyy}.xlsx";
        return GenerateExcel(filename, transacciones);
    }

    [HttpGet]
    public async Task<FileResult> GetExcelFileYear(int year)
    {
        var uid = userService.GetUserId();
        var fechaInicio = new DateTime(year, 1, 1);
        var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
        var transacciones = await repositoryTransactions.GetByUser(new TransaccionesPorUsuario()
        {
            UsuarioId = uid,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
        });

        var filename = $"Transacciones {fechaInicio:yyyy}.xlsx";
        return GenerateExcel(filename, transacciones);
    }

    [HttpGet]
    public async Task<FileResult> GetExcelFileAll()
    {
        var uid = userService.GetUserId();
        var transacciones = await repositoryTransactions.GetByUser(uid);
        var filename = $"Transacciones.xlsx";
        return GenerateExcel(filename, transacciones);
    }
    
    public async Task<IActionResult> Index(int month, int year)
    {
        var uid = userService.GetUserId();
        var modelo = await reportService.GetDetailedTransactions(uid, month, year, ViewBag);
        return View(modelo);
    }

    public async Task<IActionResult> Mensual(int year)
    {
        var uid = userService.GetUserId();

        if (year == 0)
            year = DateTime.Now.Year;

        var transacciones = await repositoryTransactions.GetMonthly(uid, year);

        var agrupado = transacciones.GroupBy(x => x.Month)
            .Select(x => new TransaccionesMensual()
            {
                Month = x.Key,
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).Sum(),
                Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).Sum(),
            })
            .ToList();

        for (int i = 1; i <= 12; i++)
        {
            var transaccion = agrupado.FirstOrDefault(x => x.Month == i);
            var referencia = new DateTime(year, i, 1);
            if (transaccion is null)
            {
                agrupado.Add(new TransaccionesMensual()
                {
                    Month = i,
                    FechaReferencia = referencia,
                });
            }
            else
            {
                transaccion.FechaReferencia = referencia;
            }
        }

        agrupado = agrupado.OrderByDescending(x => x.Month).ToList();

        var modelo = new ReporteMensualViewModel()
        {
            Transacciones = agrupado,
            Year = year
        };

        return View(modelo);
    }

    public async Task<IActionResult> Semanal(int month, int year)
    {
        var uid = userService.GetUserId();
        IEnumerable<TransaccionesSemanal> transaccionesPorSemana = await reportService.GetWeeklyReport(uid, month, year, ViewBag);
        var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new TransaccionesSemanal()
        {
            Semana = x.Key,
            Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
            Egresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault(),
        }).ToList();

        if (year == 0 || month == 0)
        {
            var today = DateTime.Now;
            year = today.Year;
            month = today.Month;
        }

        var reference = new DateTime(year, month, 1);
        var diasMes = Enumerable.Range(1, reference.AddMonths(1).AddDays(-1).Day);

        var segments = diasMes.Chunk(7).ToList();

        for (int i = 0; i < segments.Count; i++)
        {
            var semana = i + 1;
            var inicio = new DateTime(year, month, segments[i].First());
            var fin = new DateTime(year, month, segments[i].Last());
            var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

            if (grupoSemana is null)
            {
                agrupado.Add(new TransaccionesSemanal()
                {
                    Semana = semana,
                    FechaInicio = inicio,
                    FechaFin = fin
                });
            }
            else
            {
                grupoSemana.FechaInicio = inicio;
                grupoSemana.FechaFin = fin;
            }
        }

        agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

        return View(new ReporteSemanalViewModel()
        {
            Transacciones = agrupado,
            FechaReferencia = reference
        });

        //return View(modelo);
    }

    public async Task<IActionResult> Update(int id, string urlRetorno = null!)
    {
        var userId = userService.GetUserId();
        var transaccion = await repositoryTransactions.GetById(id, userId);
        if (transaccion is null)
            return RedirectToAction("NoEncontrado", "Home");

        var model = mapper.Map<TransaccionActualizarViewModel>(transaccion);
        if (model.TipoOperacionId == TipoOperacion.Gasto)
            model.MontoAnterior = transaccion.Monto * -1;
        else
            model.MontoAnterior = transaccion.Monto;

        model.CuentaAnteriorId = transaccion.CuentaId;
        model.Categorias = await GetCategories(userId, transaccion.TipoOperacionId);
        model.Cuentas = await GetAccounts(userId);
        model.UrlRetorno = urlRetorno;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TransaccionActualizarViewModel model)
    {
        var uid = userService.GetUserId();
        if (!ModelState.IsValid)
            return RedirectToAction("NoEncontrado", "Home");

        var cuenta = await repositoryAccounts.GetById(model.CuentaId, uid);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");

        var categoria = await repositoryCategory.GetById(model.CategoriaId, uid);
        if (categoria is null)
            return RedirectToAction("NoEncontrado", "Home");

        var transaccion = mapper.Map<Transaccion>(model);
        if (transaccion.TipoOperacionId == TipoOperacion.Gasto)
            transaccion.Monto *= -1;
        await repositoryTransactions.Update(transaccion, model.MontoAnterior, model.CuentaAnteriorId);

        if (string.IsNullOrEmpty(model.UrlRetorno))
            return RedirectToAction("Index");
        else
            return LocalRedirect(model.UrlRetorno);
    }

    private FileResult GenerateExcel(string name, IEnumerable<Transaccion> transactions)
    {
        var data = new DataTable("Transacciones");
        data.Columns.AddRange(new[]{
            new DataColumn("Fecha transacción"),
            new DataColumn("Cuenta"),
            new DataColumn("Categoría"),
            new DataColumn("Nota"),
            new DataColumn("Monto"),
            new DataColumn("Tipo operación"),
        });

        foreach (var transaction in transactions)
        {
            data.Rows.Add(transaction.FechaTransaccion,
                transaction.Cuenta,
                transaction.Categoria,
                transaction.Nota,
                transaction.Monto,
                transaction.TipoOperacionId);
        }

        using var workbook = new XLWorkbook();
        using var stream = new MemoryStream();
        workbook.Worksheets.Add(data);
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
    }

    private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, TipoOperacion tipoOperacion)
        => (await repositoryCategory.Get(userId, tipoOperacion)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
}