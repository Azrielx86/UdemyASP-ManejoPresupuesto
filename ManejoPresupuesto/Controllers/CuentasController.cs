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
    private readonly IRepositoryTiposCuentas repositoryTiposCuentas;
    private readonly IUserService userService;

    public CuentasController(IRepositoryTiposCuentas repositoryTiposCuentas,
        IUserService userService, IRepositoryAccounts repositoryAccounts,
        IMapper mapper)
    {
        this.repositoryTiposCuentas = repositoryTiposCuentas;
        this.userService = userService;
        this.repositoryAccounts = repositoryAccounts;
        this.mapper = mapper;
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
        if (tipoCuenta is null) return RedirectToAction("NotFoundProperty", "Home");
        if (!ModelState.IsValid)
        {
            cuenta.TiposCuenta = await GetAccountTypes(userId);
            return View(cuenta);
        }

        await repositoryAccounts.Create(cuenta);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, userId);
        if (cuenta is null)
            return RedirectToAction("NotFoundProperty", "Home");

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
            return RedirectToAction("NotFoundProperty", "Home");
        var tipoCuenta = await repositoryTiposCuentas.GetById(cuentaEditar.TipoCuentaId, userId);
        if (tipoCuenta is null)
            return RedirectToAction("NotFoundProperty", "Home");

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
            return RedirectToAction("NotFoundProperty", "Home");

        return View(cuenta);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var userId = userService.GetUserId();
        var cuenta = await repositoryAccounts.GetById(id, userId);
        if (cuenta is null)
            return RedirectToAction("NotFoundProperty", "Home");

        await repositoryAccounts.Delete(id);

        return RedirectToAction("Index");
    }

    private async Task<IEnumerable<SelectListItem>> GetAccountTypes(int userId)
        => (await repositoryTiposCuentas.GetAll(userId))
           .Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
}