using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepositoryAccounts repositoryAccounts;
        private readonly IRepositoryCategory repositoryCategory;
        private readonly IUserService userService;

        public CategoriesController(IUserService userService,
                                    IRepositoryAccounts repositoryAccounts,
                                    IRepositoryCategory repositoryCategory)
        {
            this.userService = userService;
            this.repositoryAccounts = repositoryAccounts;
            this.repositoryCategory = repositoryCategory;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            var uid = userService.GetUserId();
            categoria.UsuarioId = uid;
            await repositoryCategory.Create(categoria);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var uid = userService.GetUserId();
            var category = await repositoryCategory.GetById(id, uid);
            if (category is null)
                return RedirectToAction("NoEncontrado", "Home");
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Categoria categoriaEditar)
        {
            var uid = userService.GetUserId();
            var category = await repositoryCategory.GetById(categoriaEditar.Id, uid);
            if (category is null)
                return RedirectToAction("NoEncontrado", "Home");

            categoriaEditar.UsuarioId = uid;
            await repositoryCategory.Update(categoriaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = userService.GetUserId();
            var category = await repositoryCategory.GetById(id, uid);
            if (category is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var uid = userService.GetUserId();
            var category = await repositoryCategory.GetById(id, uid);
            if (category is null)
                return RedirectToAction("NoEncontrado", "Home");

            await repositoryCategory.Delete(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var uid = userService.GetUserId();
            var categories = await repositoryCategory.GetAll(uid);
            return View(categories);
        }
    }
}