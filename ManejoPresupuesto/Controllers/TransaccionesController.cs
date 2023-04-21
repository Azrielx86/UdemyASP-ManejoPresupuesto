using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers;

public class TransaccionesController : Controller
{
    private readonly IRepositoryAccounts repositoryAccounts;
    private readonly IRepositoryCategory repositoryCategory;
    private readonly IUserService userService;

    public TransaccionesController(IUserService userService,
        IRepositoryAccounts repositoryAccounts,
        IRepositoryCategory repositoryCategory)
    {
        this.userService = userService;
        this.repositoryAccounts = repositoryAccounts;
        this.repositoryCategory = repositoryCategory;
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

    public async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        => (await repositoryAccounts.Search(userId)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

    [HttpPost]
    public async Task<IActionResult> GetCategories([FromBody] TipoOperacion tipoOperacion)
    {
        var userId = userService.GetUserId();
        var categories = await GetCategories(userId, tipoOperacion);
        return Ok(categories);
    }

    private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, TipoOperacion tipoOperacion)
        => (await repositoryCategory.Get(userId, tipoOperacion)).Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
}