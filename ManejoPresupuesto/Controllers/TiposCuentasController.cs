using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositoryTiposCuentas repositoryTiposCuentas;
        private readonly IUserService userService;

        public TiposCuentasController(IRepositoryTiposCuentas repositoryTiposCuentas, IUserService userService)
        {
            this.repositoryTiposCuentas = repositoryTiposCuentas;
            this.userService = userService;
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
                return View(tipoCuenta);

            tipoCuenta.UsuarioId = userService.GetUserId();

            if (await repositoryTiposCuentas.Exists(tipoCuenta.Nombre!, tipoCuenta.UsuarioId))
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await repositoryTiposCuentas.Create(tipoCuenta);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = userService.GetUserId();
            var tipoCuenta = await repositoryTiposCuentas.GetById(id, usuarioId);
            if (tipoCuenta is null)
                return RedirectToAction("NotFoundProperty", "Home");
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var usuarioId = userService.GetUserId();
            var tipoCuenta = repositoryTiposCuentas.GetById(id, usuarioId);
            if (tipoCuenta is null)
                return RedirectToAction("NotFoundProperty", "Home");
            await repositoryTiposCuentas.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = userService.GetUserId();
            var tipoCuenta = await repositoryTiposCuentas.GetById(id, usuarioId);
            if (tipoCuenta is null)
                return RedirectToAction("NotFoundProperty", "Home");
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = userService.GetUserId();
            var tipoCuentaExists = await repositoryTiposCuentas.GetById(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExists is null)
            {
                return RedirectToAction("NotFoundProperty", "Home");
            }

            await repositoryTiposCuentas.Update(tipoCuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = userService.GetUserId();
            var tiposCuentas = await repositoryTiposCuentas.GetAll(usuarioId);
            return View(tiposCuentas);
        }

        [HttpGet]
        public async Task<IActionResult> TipoCuentaExists(string nombre)
        {
            var usuarioId = userService.GetUserId();
            if (await repositoryTiposCuentas.Exists(nombre, usuarioId))
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] int[] ids)
        {
            try
            {
                var usuarioId = userService.GetUserId();
                var tiposCuentas = await repositoryTiposCuentas.GetAll(usuarioId);
                var idsTiposCuentas = tiposCuentas.Select(t => t.Id);
                var idsTiposCuentasNoUserHas = ids.Except(idsTiposCuentas).ToList();
                if (idsTiposCuentasNoUserHas.Count > 0)
                    return Forbid();
                var ordered = ids.Select((value, idx) => new TipoCuenta { Id = value, Orden = idx + 1 }).AsEnumerable();

                await repositoryTiposCuentas.Order(ordered);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return Ok();
        }
    }
}