using ManejoPresupuesto.Models;
using ManejoPresupuesto.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers;

public class UsuariosController : Controller
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;

    public UsuariosController(UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public IActionResult Index() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Registro() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User() { Email = model.Email };
        var result = await userManager.CreateAsync(user, password: model.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Transacciones");
        }
        else
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Transacciones");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var resultado =
            await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                lockoutOnFailure: false);

        if (resultado.Succeeded) return RedirectToAction("Index", "Transacciones");
        else
        {
            ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
            return View(model);
        }
    }
}