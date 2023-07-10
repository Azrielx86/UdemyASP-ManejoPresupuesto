﻿using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        => (await repositoryAccounts.Search(userId)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

    [HttpPost]
    public async Task<IActionResult> GetCategories([FromBody] TipoOperacion tipoOperacion)
    {
        var userId = userService.GetUserId();
        var categories = await GetCategories(userId, tipoOperacion);
        return Ok(categories);
    }

    public async Task<IActionResult> Index(int month, int year)
    {
        var uid = userService.GetUserId();
        var modelo = await reportService.GetDetailedTransactions(uid, month, year, ViewBag);
        return View(modelo);
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

    private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, TipoOperacion tipoOperacion)
        => (await repositoryCategory.Get(userId, tipoOperacion)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
}