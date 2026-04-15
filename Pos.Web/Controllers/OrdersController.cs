using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

public class OrdersController : Controller
{
    private readonly ApiClient _api;
    public OrdersController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var orders = await _api.GetOrdersAsync() ?? new();
        return View(orders);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _api.GetOrderAsync(id);
        if (order is null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _api.GetCustomersAsync() ?? new();
        ViewBag.Products = await _api.GetProductsAsync() ?? new();
        return View(new CreateOrderViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _api.GetCustomersAsync() ?? new();
            ViewBag.Products = await _api.GetProductsAsync() ?? new();
            return View(vm);
        }
        var (success, order, error) = await _api.CreateOrderAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Hata oluştu."); ViewBag.Customers = await _api.GetCustomersAsync() ?? new(); ViewBag.Products = await _api.GetProductsAsync() ?? new(); return View(vm); }
        TempData["Success"] = "Sipariş başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Details), new { id = order!.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _api.CancelOrderAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }
}

