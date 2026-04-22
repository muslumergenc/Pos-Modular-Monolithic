using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;
using System.Security.Claims;

namespace Pos.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var (success, token, fullName, email, roles, error) = await _api.LoginAsync(vm.Email, vm.Password);
        if (!success) { ModelState.AddModelError("", error ?? "Giriş başarısız."); return View(vm); }

        // Cookie Authentication için claim listesi oluştur (token da claim olarak saklanıyor)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, fullName ?? email!),
            new Claim(ClaimTypes.Email, email!),
            new Claim("JwtToken", token!),
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "PosAuthCookie");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("PosAuthCookie", principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

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

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("PosAuthCookie");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();
}

