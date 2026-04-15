using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

public class CustomersController : Controller
{
    private readonly ApiClient _api;
    public CustomersController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var customers = await _api.GetCustomersAsync() ?? new();
        return View(customers);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await _api.GetCustomerAsync(id);
        if (customer is null) return NotFound();
        return View(customer);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateCustomerViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (success, error) = await _api.CreateCustomerAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Hata oluştu."); return View(vm); }
        TempData["Success"] = "Müşteri başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }
}

