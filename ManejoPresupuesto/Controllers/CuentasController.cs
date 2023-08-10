using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers;

public class CuentasController : Controller
{
    private readonly IRepositoryAccounts repositoryAccounts;
    private readonly IMapper mapper;
    private readonly IRepositoryTransactions repositoryTransactions;
    private readonly IReportService reportService;
    private readonly IRepositoryTiposCuentas repositoryTiposCuentas;
    private readonly IUserService userService;

    public CuentasController(IRepositoryTiposCuentas repositoryTiposCuentas,
        IUserService userService, IRepositoryAccounts repositoryAccounts,
        IMapper mapper, IRepositoryTransactions repositoryTransactions,
        IReportService reportService)
    {
        this.repositoryTiposCuentas = repositoryTiposCuentas;
        this.userService = userService;
        this.repositoryAccounts = repositoryAccounts;
        this.mapper = mapper;
        this.repositoryTransactions = repositoryTransactions;
        this.reportService = reportService;
    }

    public async Task<IActionResult> Create()
    {
        var userId = userService.GetUserId();
        var tiposCuentas = await repositoryTiposCuentas.GetAll(userId);

        var model = new CuentasCreacionViewModel
        {
            TiposCuenta = await GetAccountTypes(userId)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CuentasCreacionViewModel cuenta)
    {
        var userId = userService.GetUserId();
        var tipoCuenta = repositoryTiposCuentas.GetById(cuenta.TipoCuentaId, userId);
        if (tipoCuenta is null) return RedirectToAction("NoEncontrado", "Home");
        if (!ModelState.IsValid)
        {
            cuenta.TiposCuenta = await GetAccountTypes(userId);
            return View(cuenta);
        }

        await repositoryAccounts.Create(cuenta);
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Obtiene los detalles de las transacciones una cuenta
    /// </summary>
    /// <param name="id">ID De la cuenta</param>
    /// <param name="month">Mes</param>
    /// <param name="year">Año</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id, int month, int year)
    {
        var uid = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, uid);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");
        ViewBag.cuenta = cuenta.Nombre;
        var modelo = await reportService.GetTransactionReportByUser(uid, id, month, year, ViewBag);

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, userId);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");

        var model = mapper.Map<CuentasCreacionViewModel>(cuenta);

        model.TiposCuenta = await GetAccountTypes(userId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CuentasCreacionViewModel cuentaEditar)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(cuentaEditar.Id, userId);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");
        var tipoCuenta = await repositoryTiposCuentas.GetById(cuentaEditar.TipoCuentaId, userId);
        if (tipoCuenta is null)
            return RedirectToAction("NoEncontrado", "Home");

        await repositoryAccounts.Update(cuentaEditar);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userService.GetUserId();
        var cuentasConTipoCuentas = await repositoryAccounts.Search(userId);

        var model = cuentasConTipoCuentas
            .GroupBy(x => x.TipoCuenta)
            .Select(group => new AccountIndexViewModel
            {
                TipoCuenta = group.Key,
                Cuentas = group.AsEnumerable()
            }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, userId);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");

        return View(cuenta);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, userId);
        if (cuenta is null)
            return RedirectToAction("NoEncontrado", "Home");

        await repositoryAccounts.Delete(id);

        return RedirectToAction("Index");
    }

    private async Task<IEnumerable<SelectListItem>> GetAccountTypes(int userId)
        => (await repositoryTiposCuentas.GetAll(userId))
           .Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
}