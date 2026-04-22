using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pos.Web.Models;

namespace Pos.Web.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void SetAuthHeader()
    {
        // Token'ı Cookie claim'inden oku (Session yerine - daha güvenilir)
        var token = _httpContextAccessor.HttpContext?.User?.FindFirst("JwtToken")?.Value;
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    private async Task<(bool Success, T? Data, string? Error)> PostAsync<T>(string url, object body)
    {
        SetAuthHeader();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (true, JsonSerializer.Deserialize<T>(json, _jsonOptions), null);
        return (false, default, json);
    }

    private async Task<(bool Success, string? Error)> DeleteAsync(string url)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync(url);
        return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    // ── Auth ─────────────────────────────────────────────────
    public async Task<(bool Success, string? Token, string? FullName, string? Email, IList<string> Roles, string? Error)> LoginAsync(string email, string password)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/login", content);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return (false, null, null, null, Array.Empty<string>(), json);
        var data = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
        var token = data.GetProperty("token").GetString();
        var fullName = data.TryGetProperty("fullName", out var fn) ? fn.GetString() : null;
        var emailVal = data.TryGetProperty("email", out var em) ? em.GetString() : null;
        var roles = data.TryGetProperty("roles", out var r)
            ? r.EnumerateArray().Select(x => x.GetString()!).ToList()
            : new List<string>();
        return (true, token, fullName, emailVal, roles, null);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterViewModel vm)
    {
        var (success, _, error) = await PostAsync<object>("/api/auth/register", new
        {
            fullName = vm.FullName,
            email = vm.Email,
            password = vm.Password,
            confirmPassword = vm.ConfirmPassword
        });
        return (success, error);
    }

    // ── Categories ──────────────────────────────────────────
    public Task<List<CategoryViewModel>?> GetCategoriesAsync() =>
        GetAsync<List<CategoryViewModel>>("/api/categories");

    // ── Products ────────────────────────────────────────────
    public Task<List<ProductViewModel>?> GetProductsAsync(Guid? categoryId = null) =>
        GetAsync<List<ProductViewModel>>(categoryId.HasValue ? $"/api/products?categoryId={categoryId}" : "/api/products");

    public Task<ProductViewModel?> GetProductAsync(Guid id) =>
        GetAsync<ProductViewModel>($"/api/products/{id}");

    public async Task<(bool Success, string? Error)> CreateProductAsync(CreateProductViewModel vm)
    {
        var (success, _, error) = await PostAsync<object>("/api/products", new
        {
            name = vm.Name, description = vm.Description,
            price = vm.Price, stock = vm.Stock, categoryId = vm.CategoryId
        });
        return (success, error);
    }

    // ── Customers ────────────────────────────────────────────
    public Task<List<CustomerViewModel>?> GetCustomersAsync() =>
        GetAsync<List<CustomerViewModel>>("/api/customers");

    public Task<CustomerViewModel?> GetCustomerAsync(Guid id) =>
        GetAsync<CustomerViewModel>($"/api/customers/{id}");

    public async Task<(bool Success, string? Error)> CreateCustomerAsync(CreateCustomerViewModel vm)
    {
        var (success, _, error) = await PostAsync<object>("/api/customers", new
        {
            fullName = vm.FullName, email = vm.Email,
            phone = vm.Phone, address = vm.Address
        });
        return (success, error);
    }

    // ── Orders ───────────────────────────────────────────────
    public Task<List<OrderViewModel>?> GetOrdersAsync() =>
        GetAsync<List<OrderViewModel>>("/api/orders");

    public Task<OrderViewModel?> GetOrderAsync(Guid id) =>
        GetAsync<OrderViewModel>($"/api/orders/{id}");

    public async Task<(bool Success, OrderViewModel? Order, string? Error)> CreateOrderAsync(CreateOrderViewModel vm)
    {
        var (success, data, error) = await PostAsync<OrderViewModel>("/api/orders", new
        {
            customerId = vm.CustomerId,
            notes = vm.Notes,
            tableId = vm.TableId,
            items = vm.Items.Select(i => new { productId = i.ProductId, quantity = i.Quantity })
        });
        return (success, data, error);
    }

    public async Task<(bool Success, string? Error)> CancelOrderAsync(Guid id)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsync($"/api/orders/{id}/cancel", null);
        return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    // ── Payments ─────────────────────────────────────────────
    public Task<List<PaymentViewModel>?> GetPaymentsAsync() =>
        GetAsync<List<PaymentViewModel>>("/api/payments");

    public Task<PaymentViewModel?> GetPaymentAsync(Guid id) =>
        GetAsync<PaymentViewModel>($"/api/payments/{id}");

    public async Task<(bool Success, PaymentViewModel? Payment, string? Error)> ProcessPaymentAsync(ProcessPaymentViewModel vm)
    {
        var (success, data, error) = await PostAsync<PaymentViewModel>("/api/payments/process", new
        {
            orderId = vm.OrderId, amount = vm.Amount,
            method = vm.Method, notes = vm.Notes
        });
        return (success, data, error);
    }

    public async Task<(bool Success, string? Error)> RefundAsync(Guid paymentId)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsync($"/api/payments/{paymentId}/refund", null);
        return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    // ── Tables ───────────────────────────────────────────────
    public Task<List<TableViewModel>?> GetTablesAsync() =>
        GetAsync<List<TableViewModel>>("/api/tables");

    public Task<TableViewModel?> GetTableAsync(Guid id) =>
        GetAsync<TableViewModel>($"/api/tables/{id}");

    public async Task<(bool Success, string? Error)> CreateTableAsync(CreateTableViewModel vm)
    {
        var (success, _, error) = await PostAsync<object>("/api/tables", new
        {
            number = vm.Number, capacity = vm.Capacity, label = vm.Label
        });
        return (success, error);
    }

    public async Task<(bool Success, string? Error)> UpdateTableStatusAsync(Guid id, int status)
    {
        SetAuthHeader();
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { status }),
            System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"/api/tables/{id}/status", content);
        return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string? Error)> DeleteTableAsync(Guid id)
    {
        return await DeleteAsync($"/api/tables/{id}");
    }

    public async Task<(bool Success, string? Error)> UpdateOrderStatusAsync(Guid id, int status)
    {
        SetAuthHeader();
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { status }),
            System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"/api/orders/{id}/status", content);
        return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Bankadan gelen callback parametrelerini API'ye iletir ve ödeme sonucunu alır.
    /// </summary>
    public async Task<(bool Success, string? Error)> ProcessGatewayCallbackAsync(
        Guid paymentId, Dictionary<string, string> parameters)
    {
        SetAuthHeader();
        var content = new StringContent(
            JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            $"/api/payments/{paymentId}/callback", content);
        return (response.IsSuccessStatusCode,
                response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, GatewayMarkupViewModel? Data, string? Error)> InitiateGatewayPaymentAsync(
        GatewayPayViewModel vm, string callbackBaseUrl)
    {
        var (success, data, error) = await PostAsync<GatewayMarkupViewModel>("/api/payments/gateway/initiate", new
        {
            orderId         = vm.OrderId,
            currency        = vm.Currency,
            language        = vm.Language,
            cardNumber      = vm.CardNumber,
            cvv             = vm.Cvv,
            expiryDateYear  = vm.ExpiryDateYear,
            expiryDateMonth = vm.ExpiryDateMonth,
            requestIp       = "127.0.0.1",
            email           = vm.Email,
            cardHolderName  = vm.CardHolderName,
            callbackBaseUrl = callbackBaseUrl   // ← Pos.Web'in gerçek adresi
        });
        return (success, data, error);
    }
}
