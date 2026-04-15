using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models;
using Pos.Web.Services;

namespace Pos.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly ApiClient _api;
    public PaymentsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var payments = await _api.GetPaymentsAsync() ?? new();
        return View(payments);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _api.GetPaymentAsync(id);
        if (payment is null) return NotFound();
        return View(payment);
    }

    [HttpGet]
    public async Task<IActionResult> Process(Guid orderId)
    {
        var order = await _api.GetOrderAsync(orderId);
        if (order is null) return NotFound();
        var vm = new ProcessPaymentViewModel { OrderId = orderId, Amount = order.TotalAmount };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Process(ProcessPaymentViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (success, payment, error) = await _api.ProcessPaymentAsync(vm);
        if (!success) { ModelState.AddModelError("", error ?? "Ödeme başarısız."); return View(vm); }
        TempData["Success"] = $"Ödeme başarılı! Referans: {payment!.TransactionReference}";
        return RedirectToAction(nameof(Details), new { id = payment.Id });
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> Refund(Guid id)
    {
        await _api.RefundAsync(id);
        TempData["Success"] = "İade işlemi başarıyla tamamlandı.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ── 3D Secure Gateway Ödeme ──────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GatewayPay(Guid orderId)
    {
        var order = await _api.GetOrderAsync(orderId);
        if (order is null) return NotFound();
        var vm = new GatewayPayViewModel
        {
            OrderId     = orderId,
            Amount      = order.TotalAmount,
            OrderNumber = order.Id.ToString()[..8].ToUpper()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GatewayPay(GatewayPayViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var (success, data, error) = await _api.InitiateGatewayPaymentAsync(
            vm,
            // Hangi portta çalışıyor olursa olsun kendi adresini dinamik al
            callbackBaseUrl: $"{Request.Scheme}://{Request.Host}");
        if (!success)
        {
            ModelState.AddModelError("", error ?? "3D Secure ödeme başlatılamadı.");
            return View(vm);
        }

        // Bankaya yönlendirme formunu otomatik gönder
        return View("GatewayRedirect", data);
    }

    /// <summary>
    /// Garanti Bankası 3D Secure callback endpoint'i.
    /// Banka, kullanıcı tarayıcısını bu adrese POST ile yönlendirir.
    /// Explicit route: /Payments/GatewayCallback/{paymentId}
    /// </summary>
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]   // Banka kendi formuyla POST eder, CSRF token içermez
    [HttpPost("Payments/GatewayCallback/{paymentId:guid}")]
    public async Task<IActionResult> GatewayCallback(Guid paymentId, IFormCollection form)
    {
        var parameters = form.ToDictionary(k => k.Key, v => v.Value.ToString());
        var (success, error) = await _api.ProcessGatewayCallbackAsync(paymentId, parameters);

        if (success)
        {
            TempData["PaymentId"] = paymentId.ToString();
            return RedirectToAction(nameof(CallbackSuccess), new { paymentId });
        }

        TempData["PaymentId"] = paymentId.ToString();
        TempData["Error"]     = error;
        return RedirectToAction(nameof(CallbackFail), new { paymentId });
    }

    /// <summary>Ödeme başarılı — sonuç sayfası</summary>
    [AllowAnonymous]
    public IActionResult CallbackSuccess(Guid paymentId)
    {
        ViewData["PaymentId"] = paymentId;
        ViewData["Success"]   = true;
        return View("CallbackResult");
    }

    /// <summary>Ödeme başarısız — sonuç sayfası</summary>
    [AllowAnonymous]
    public IActionResult CallbackFail(Guid paymentId)
    {
        ViewData["PaymentId"] = paymentId;
        ViewData["Success"]   = false;
        ViewData["Error"]     = TempData["Error"];
        return View("CallbackResult");
    }
}
