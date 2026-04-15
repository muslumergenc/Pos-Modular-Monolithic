using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (success, token, error) = await _api.LoginAsync(vm.Email, vm.Password);
        if (!success) { ModelState.AddModelError("", error ?? "Giriş başarısız."); return View(vm); }
        HttpContext.Session.SetString("JwtToken", token!);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (success, error) = await _api.RegisterAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Kayıt başarısız."); return View(vm); }
        TempData["Success"] = "Kayıt başarılı. Giriş yapabilirsiniz.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        return RedirectToAction(nameof(Login));
    }
}

