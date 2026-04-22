using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

[Authorize(Roles = "Admin,Waiter,Cashier")]
public class TablesController : Controller
{
    private readonly ApiClient _api;
    public TablesController(ApiClient api) => _api = api;

    // Masa planı (grid)
    public async Task<IActionResult> Index()
    {
        var tables = await _api.GetTablesAsync() ?? new();
        return View(tables);
    }

    // Masa detayı + o masanın aktif siparişleri
    public async Task<IActionResult> Details(Guid id)
    {
        var table = await _api.GetTableAsync(id);
        if (table is null) return NotFound();

        var allOrders = await _api.GetOrdersAsync() ?? new();
        var tableOrders = allOrders
            .Where(o => o.TableId == id && o.Status < 4) // Tamamlanmamış siparişler
            .ToList();

        ViewBag.Orders = tableOrders;
        return View(table);
    }

    // Masa oluştur (Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new CreateTableViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateTableViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (success, error) = await _api.CreateTableAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Hata oluştu."); return View(vm); }
        TempData["Success"] = $"Masa {vm.Number} oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // Masa durumu güncelle (AJAX)
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid id, int status)
    {
        await _api.UpdateTableStatusAsync(id, status);
        return RedirectToAction(nameof(Index));
    }

    // Masadan sipariş al
    [HttpGet]
    public async Task<IActionResult> NewOrder(Guid id)
    {
        var table = await _api.GetTableAsync(id);
        if (table is null) return NotFound();

        ViewBag.Table = table;
        ViewBag.Customers = await _api.GetCustomersAsync() ?? new();
        ViewBag.Products = await _api.GetProductsAsync() ?? new();

        var vm = new CreateOrderViewModel { TableId = id };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> NewOrder(CreateOrderViewModel vm)
    {
        if (!ModelState.IsValid || vm.Items.Count == 0)
        {
            var table = await _api.GetTableAsync(vm.TableId!.Value);
            ViewBag.Table = table;
            ViewBag.Customers = await _api.GetCustomersAsync() ?? new();
            ViewBag.Products = await _api.GetProductsAsync() ?? new();
            ModelState.AddModelError("", "En az bir ürün eklemelisiniz.");
            return View(vm);
        }

        var (success, order, error) = await _api.CreateOrderAsync(vm);
        if (!success)
        {
            var table = await _api.GetTableAsync(vm.TableId!.Value);
            ViewBag.Table = table;
            ViewBag.Customers = await _api.GetCustomersAsync() ?? new();
            ViewBag.Products = await _api.GetProductsAsync() ?? new();
            ModelState.AddModelError("", error ?? "Sipariş oluşturulamadı.");
            return View(vm);
        }

        TempData["Success"] = "Sipariş mutfağa iletildi!";
        return RedirectToAction(nameof(Details), new { id = vm.TableId });
    }

    // Masa sil (Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteTableAsync(id);
        TempData["Success"] = "Masa silindi.";
        return RedirectToAction(nameof(Index));
    }
}
