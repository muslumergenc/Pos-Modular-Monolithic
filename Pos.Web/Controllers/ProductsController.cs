using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly ApiClient _api;
    public ProductsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var products = await _api.GetProductsAsync() ?? new();
        return View(products);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _api.GetProductAsync(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _api.GetCategoriesAsync() ?? new();
        return View(new CreateProductViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _api.GetCategoriesAsync() ?? new();
            return View(vm);
        }
        var (success, error) = await _api.CreateProductAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Hata oluştu."); ViewBag.Categories = await _api.GetCategoriesAsync() ?? new(); return View(vm); }
        TempData["Success"] = "Ürün başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }
}

