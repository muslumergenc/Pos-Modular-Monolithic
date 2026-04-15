# 🏪 POS Sistemi — Sistem Dokümantasyonu

> **Versiyon**: 2.0 · **Framework**: .NET 9.0 · **Tarih**: Nisan 2026  
> **Mimari**: Modular Monolithic + Clean Architecture

---

## 📋 İçindekiler

1. [Proje Amacı](#-proje-amacı)
2. [Mimari Genel Bakış](#-mimari-genel-bakış)
3. [Solution Yapısı](#-solution-yapısı)
4. [Modüller](#-modüller)
5. [Teknoloji Yığını](#-teknoloji-yığını)
6. [Garanti Bankası 3D Secure Entegrasyonu](#-garanti-bankası-3d-secure-entegrasyonu)
7. [Veri Akışı](#-veri-akışı)
8. [API Endpoint'leri](#-api-endpointleri)
9. [Frontend (Pos.Web)](#-frontend-posweb)
10. [Güvenlik](#-güvenlik)
11. [Yapılandırma](#-yapılandırma)
12. [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma)
13. [Veritabanı](#-veritabanı)
14. [Geliştirme Prensipleri](#-geliştirme-prensipleri)
15. [Ölçeklenebilirlik](#-ölçeklenebilirlik)

---

## 🎯 Proje Amacı

**POS (Point of Sale) Sistemi**, perakende ve hizmet sektöründe kullanılmak üzere geliştirilmiş, tam entegre bir satış noktası uygulamasıdır.

### Temel Yetenekler

| Yetenek | Açıklama |
|---|---|
| 🛒 **Sipariş Yönetimi** | Ürün seçimi, sipariş oluşturma, durum takibi |
| 💳 **Ödeme İşleme** | Nakit, kredi kartı, banka kartı, dijital ödeme |
| 🔐 **3D Secure Ödeme** | Garanti Bankası entegrasyonu ile güvenli kart ödemesi |
| 📦 **Ürün Kataloğu** | Kategori ve ürün yönetimi, stok takibi |
| 👥 **Müşteri Yönetimi** | Müşteri kaydı ve geçmiş takibi |
| 🔑 **Kimlik Yönetimi** | JWT tabanlı kullanıcı doğrulama ve yetkilendirme |

---

## 🏛️ Mimari Genel Bakış

Proje iki temel mimari prensip üzerine inşa edilmiştir:

### Modular Monolith

Tek bir deploy birimi içinde **birbirinden bağımsız**, gevşek bağlı modüller. Her modül kendi veritabanı şemasına, iş mantığına ve API'sine sahiptir. Microservice karmaşıklığı olmadan modüler geliştirme sağlar.

### Clean Architecture

Her modül içinde bağımlılıklar **içten dışa** akar:

```
Domain  ←  Application  ←  Infrastructure
                ↑
          Presentation
```

- **Domain**: Dış dünyadan tamamen bağımsız, saf iş mantığı
- **Application**: Orchestration, CQRS, Use Case'ler
- **Infrastructure**: Veritabanı, dış servisler, implementasyonlar
- **Presentation**: HTTP katmanı, Controller'lar

### Sistem Katmanları

```
┌──────────────────────────────────────────────────────────────────┐
│                         Pos.Web (MVC)                             │
│               Kullanıcı Arayüzü · Razor Views                    │
│         Session (JWT) · ApiClient (HttpClient wrapper)           │
└─────────────────────────────┬────────────────────────────────────┘
                              │ HTTPS + JSON + JWT Bearer
┌─────────────────────────────▼────────────────────────────────────┐
│                      Pos.EndPoint (API)                           │
│          ASP.NET Core Web API · Swagger · CORS · JWT             │
└──┬──────────────┬──────────────┬──────────────┬──────────────────┘
   │              │              │              │
┌──▼──┐      ┌───▼──┐      ┌────▼──┐      ┌────▼──┐
│ 🔑  │      │  📦  │      │  👥   │      │  🛍️  │      ...
│ Identity   │Catalog│   │Customers│    │ Orders │
└─────┘      └──────┘      └───────┘      └───────┘
                                                     ┌──────────┐
                                                     │ 💳       │
                                                     │Payments  │
                                                     └────┬─────┘
                                                          │
                                               ┌──────────▼──────────┐
                                               │  Pos.Providers      │
                                               │  .GarantiBank       │
                                               │  (3D Secure)        │
                                               └─────────────────────┘
┌──────────────────────────────────────────────────────────────────┐
│                       Pos.Shared (Kernel)                         │
│         BaseEntity · IRepository · Result<T> · IDomainEvent      │
└──────────────────────────────────────────────────────────────────┘
```

---

## 📂 Solution Yapısı

```
Pos.sln
│
├── Pos.Shared/                          # Shared Kernel — tüm modüller kullanır
│   ├── Domain/
│   │   ├── BaseEntity.cs               # Id, CreatedAt, UpdatedAt, DomainEvents
│   │   └── IDomainEvent.cs
│   ├── Abstractions/
│   │   └── IRepository<T>.cs           # Generic CRUD interface
│   └── Common/
│       └── Result<T>.cs                # Type-safe başarı/hata sarmalayıcı
│
├── Pos.EndPoint/                        # API Host — tüm modülleri birleştirir
│   ├── Program.cs                       # DI, JWT, Swagger, Migration, CORS
│   ├── appsettings.json                 # Connection string, JWT, GarantiBank config
│   └── appsettings.Development.json     # Geliştirme ortamı ayarları
│
├── Pos.Web/                             # MVC Frontend
│   ├── Controllers/                     # MVC Controller'lar
│   ├── Views/                           # Razor View'lar
│   │   └── Payments/
│   │       ├── GatewayPay.cshtml        # 3D Secure kart giriş formu
│   │       ├── GatewayRedirect.cshtml   # Bankaya otomatik yönlendirme
│   │       └── CallbackResult.cshtml    # Ödeme sonuç sayfası
│   ├── Models/ViewModels.cs             # View model sınıfları
│   ├── Services/ApiClient.cs            # HTTP istemcisi
│   └── appsettings.json                 # ApiSettings:BaseUrl
│
├── Pos.Providers.GarantiBank/           # Garanti Bankası 3D Secure Provider
│   ├── GarantiBankOptions.cs            # appsettings'ten okunan yapılandırma
│   ├── IGarantiBankPaymentProvider.cs   # Sözleşme (interface)
│   ├── GarantiBankPaymentProvider.cs    # Implementasyon
│   ├── Helper.cs                        # SHA1/SHA512 hash, 3D hash hesaplama
│   ├── Constants.cs                     # HTML form şablonu, mdStatus kodları
│   ├── Startup.cs                       # DI kaydı (RegisterGarantiBank)
│   ├── Models/                          # Request/Response modelleri
│   │   ├── PreparePaymentRequest.cs
│   │   ├── PreparePaymentResponse.cs
│   │   ├── BankCallbackResponse.cs
│   │   └── BankCallbackParameters.cs
│   ├── Common/
│   │   └── OperationResult.cs           # Provider-spesifik sonuç tipi
│   ├── Enums/
│   │   ├── ThirdPartyProvider.cs
│   │   └── PaymentGatewayMode.cs
│   └── Helpers/
│       ├── EnvironmentHelper.cs
│       └── NumberHelpers.cs
│
└── Modules/
    ├── Identity/
    │   ├── Pos.Modules.Identity.Domain/
    │   ├── Pos.Modules.Identity.Application/
    │   ├── Pos.Modules.Identity.Infrastructure/
    │   └── Pos.Modules.Identity.Presentation/
    ├── Catalog/
    │   └── (4 katman)
    ├── Customers/
    │   └── (4 katman)
    ├── Orders/
    │   └── (4 katman)
    └── Payments/
        ├── Pos.Modules.Payments.Domain/
        ├── Pos.Modules.Payments.Application/
        │   ├── Commands/
        │   │   ├── PaymentCommands.cs          # ProcessPayment, Refund
        │   │   └── GatewayPaymentCommands.cs   # InitiateGateway, ProcessCallback
        │   ├── Handlers/
        │   │   ├── PaymentHandlers.cs
        │   │   └── GatewayPaymentHandlers.cs
        │   ├── Interfaces/
        │   │   ├── IPaymentInterfaces.cs        # IPaymentRepository, IOrderStatusService
        │   │   └── IGatewayPaymentService.cs    # 3D Secure servis sözleşmesi
        │   └── DTOs/
        │       ├── PaymentDtos.cs
        │       └── GatewayPaymentDtos.cs
        ├── Pos.Modules.Payments.Infrastructure/
        │   ├── Services/
        │   │   ├── OrderStatusService.cs
        │   │   └── GatewayPaymentService.cs    # GarantiBank provider sarmalayıcı
        │   └── Extensions/
        │       └── PaymentsInfrastructureExtensions.cs  # RegisterGarantiBank dahil
        └── Pos.Modules.Payments.Presentation/
            └── Controllers/PaymentsController.cs
```

---

## 🧩 Modüller

### 🔑 Identity Modülü

**Amaç**: Kullanıcı kimlik doğrulama ve JWT token yönetimi

| Bileşen | Detay |
|---|---|
| Entity | `AppUser` (IdentityUser türevi) |
| Komutlar | `RegisterCommand`, `LoginCommand` |
| Servisler | `JwtTokenService` — token üretimi |
| Endpoints | `POST /api/auth/register`, `POST /api/auth/login` |

---

### 📦 Catalog Modülü

**Amaç**: Ürün ve kategori yönetimi, stok takibi

| Bileşen | Detay |
|---|---|
| Entity'ler | `Category`, `Product` |
| İş Kuralları | Fiyat > 0 zorunluluğu, stok negatif olamaz |
| Komutlar | Create, Update, Delete (Category & Product) |
| Sorgular | GetAll (filtreli), GetById |
| Endpoints | `/api/categories`, `/api/products` |

---

### 👥 Customers Modülü

**Amaç**: Müşteri kaydı ve bilgi yönetimi

| Bileşen | Detay |
|---|---|
| Entity | `Customer` |
| Komutlar | Create, Update, Deactivate |
| Endpoints | `/api/customers` |

---

### 🛍️ Orders Modülü

**Amaç**: Sipariş oluşturma, durum takibi ve yönetimi

| Bileşen | Detay |
|---|---|
| Entity'ler | `Order` (Aggregate Root), `OrderItem` |
| Durumlar | `Pending → Processing → Completed / Cancelled` |
| Cross-Module | `IProductService` (Catalog'dan), `ICustomerService` (Customers'dan) |
| Komutlar | CreateOrder, UpdateStatus, CancelOrder |
| Endpoints | `/api/orders` |

**Sipariş oluşturma iş akışı:**
```
Müşteri doğrulandı? → Ürünler mevcut mu? → Stok yeterli mi?
        ↓                    ↓                    ↓
Order.Create()  →  AddItem × n  →  StockDecrease  →  Kayıt
```

---

### 💳 Payments Modülü

**Amaç**: Ödeme işleme, 3D Secure gateway entegrasyonu ve iade yönetimi

| Bileşen | Detay |
|---|---|
| Entity | `Payment` |
| Durumlar | `Pending → Completed / Failed / Refunded` |
| Yöntemler | `Cash, CreditCard, DebitCard, Digital` |
| Ödeme Türleri | Standart ödeme + 3D Secure (Garanti Bankası) |
| Cross-Module | `IOrderStatusService` (Orders'dan) |

---

## 🏦 Garanti Bankası 3D Secure Entegrasyonu

### Mimarisi

```
Pos.Modules.Payments.Application
    └── IGatewayPaymentService (interface — altyapıdan bağımsız)
         ↑ implement eder
Pos.Modules.Payments.Infrastructure
    └── GatewayPaymentService
         └── IGarantiBankPaymentProvider (inject)
              ↑ implement eder
Pos.Providers.GarantiBank
    └── GarantiBankPaymentProvider
```

**Katman izolasyonu**: Application katmanı `IGatewayPaymentService`'i bilir, `GarantiBankPaymentProvider`'ı asla doğrudan görmez. Yarın farklı bir banka entegre edilse Application kodu değişmez.

---

### 3D Secure Ödeme Akışı

```
 Kullanıcı          Pos.Web            Pos.EndPoint          GarantiBank
    │                  │                    │                     │
    │──GatewayPay──►   │                    │                     │
    │   (GET form)     │                    │                     │
    │                  │                    │                     │
    │──Kart bilgisi──► │                    │                     │
    │   (POST form)    │                    │                     │
    │                  │──POST /api/────►   │                     │
    │                  │  gateway/initiate  │                     │
    │                  │  + CallbackBaseUrl │                     │
    │                  │                    │──PreparePayment──►  │
    │                  │                    │  (HTML form markup) │
    │                  │◄──HTML Markup──── │                     │
    │◄──GatewayRedirect│                    │                     │
    │   (auto-submit)  │                    │                     │
    │                  │                    │                     │
    │────────── Banka 3D Secure sayfası ─────────────────────►  │
    │◄──────── SMS OTP doğrulaması ──────────────────────────── │
    │                  │                    │                     │
    │◄── POST Callback ─│                   │                     │
    │  (Banka form POST │                   │                     │
    │  → Pos.Web)       │                   │                     │
    │                  │──POST callback──►  │                     │
    │                  │  parameters (JSON) │                     │
    │                  │                    │──BankCallback──►   │
    │                  │                    │  (hash verify)      │
    │                  │                    │──Payment.Update──  │
    │                  │◄──Success/Fail──── │                     │
    │◄──CallbackResult  │                    │                     │
    │   (HTML Sayfası)  │                    │                     │
```

---

### Callback URL Yönetimi

Callback URL'i **dinamik** olarak Pos.Web'in kendi adresinden üretilir:

```csharp
// Pos.Web/Controllers/PaymentsController.cs
callbackBaseUrl: $"{Request.Scheme}://{Request.Host}"
// → https://localhost:7601/Payments/GatewayCallback/{paymentId}
```

**appsettings'teki şablon** (path kısmı):
```json
"SuccessUrl": "https://[web-host]/Payments/GatewayCallback/:paymentId",
"FailUrl":    "https://[web-host]/Payments/GatewayCallback/:paymentId"
```

Provider, `CallbackBaseUrl` sağlanmışsa appsettings'teki base'i ezer, path şablonunu korur.

---

### Hash Güvenliği

3D Secure form hash'i **SHA-512** ile hesaplanır:

```
HASH = SHA512(terminalId + orderId + amount + currencyCode +
              successUrl + failUrl + txnType + installment +
              storeKey + hashedPassword)

hashedPassword = SHA1(provisionPassword + terminalId[9 hane])
```

Geliştirme ortamında (`SkipHashVerification: true`) hash doğrulaması atlanır.

---

### mdStatus Kodları

| Kod | Anlam | Sonuç |
|---|---|---|
| `1` | Doğrulama başarılı | ✅ Ödeme devam eder |
| `0` | İmza hatası | ❌ Reddedilir |
| `2` | Kart 3D Secure uygun değil | ❌ Reddedilir |
| `3` | Kart sağlayıcı desteklemiyor | ❌ Reddedilir |
| `4` | Doğrulama girişimi | ❌ Reddedilir |
| `5` | Doğrulanamıyor | ❌ Reddedilir |
| `6` | 3D Secure hatası | ❌ Reddedilir |
| `7` | Sistem hatası | ❌ Reddedilir |
| `8` | Geçersiz kart numarası | ❌ Reddedilir |
| `9` | İşyeri kayıtlı değil | ❌ Reddedilir |

---

## 🔄 Veri Akışı

### CQRS Pattern

```
HTTP Request
    ↓
Controller  →  IMediator.Send(Command / Query)
                  ↓
             Handler
             ├── Command: Repository'ye yazar, domain logic çalıştırır
             └── Query: Repository'den okur
                  ↓
             Result<T>
                  ↓
Controller  →  IActionResult (200 / 400 / 404)
```

### Modüller Arası İletişim

Modüller **sadece Infrastructure katmanında** ve **sadece interface üzerinden** iletişim kurar. Doğrudan birbirinin DbContext'ine yazamazlar.

```
Orders.Infrastructure.Services.ProductService
    → ICatalogDbContext (read-only)   ← Catalog ürün bilgisi

Orders.Infrastructure.Services.CustomerService
    → ICustomersDbContext (read-only) ← Customers müşteri adı

Payments.Infrastructure.Services.OrderStatusService
    → OrdersDbContext                 ← Sipariş durumu güncelle
```

---

## 📡 API Endpoint'leri

**Base URL**: `https://localhost:[API_PORT]`  
**Auth**: `Authorization: Bearer {JWT_TOKEN}` (✅ gerekli olanlar için)

### 🔑 Identity (`/api/auth`)

| Method | Endpoint | Auth | Açıklama |
|---|---|---|---|
| POST | `/api/auth/register` | ❌ | Kullanıcı kaydı |
| POST | `/api/auth/login` | ❌ | Giriş → JWT token döner |

### 📦 Catalog (`/api/categories`, `/api/products`)

| Method | Endpoint | Auth | Açıklama |
|---|---|---|---|
| GET | `/api/categories` | ❌ | Tüm kategoriler |
| GET | `/api/categories/{id}` | ❌ | Kategori detayı |
| POST | `/api/categories` | ✅ | Yeni kategori |
| PUT | `/api/categories/{id}` | ✅ | Kategori güncelle |
| DELETE | `/api/categories/{id}` | ✅ | Kategori sil |
| GET | `/api/products` | ❌ | Tüm ürünler (filtreli) |
| GET | `/api/products/{id}` | ❌ | Ürün detayı |
| POST | `/api/products` | ✅ | Yeni ürün |
| PUT | `/api/products/{id}` | ✅ | Ürün güncelle |
| DELETE | `/api/products/{id}` | ✅ | Ürün sil |

### 👥 Customers (`/api/customers`)

| Method | Endpoint | Auth | Açıklama |
|---|---|---|---|
| GET | `/api/customers` | ✅ | Tüm müşteriler |
| GET | `/api/customers/{id}` | ✅ | Müşteri detayı |
| POST | `/api/customers` | ✅ | Yeni müşteri |
| PUT | `/api/customers/{id}` | ✅ | Müşteri güncelle |
| DELETE | `/api/customers/{id}` | ✅ | Müşteri sil |

### 🛍️ Orders (`/api/orders`)

| Method | Endpoint | Auth | Açıklama |
|---|---|---|---|
| GET | `/api/orders` | ✅ | Tüm siparişler |
| GET | `/api/orders/{id}` | ✅ | Sipariş detayı |
| POST | `/api/orders` | ✅ | Yeni sipariş |
| PATCH | `/api/orders/{id}/status` | ✅ | Durum güncelle |
| POST | `/api/orders/{id}/cancel` | ✅ | İptal et |

### 💳 Payments (`/api/payments`)

| Method | Endpoint | Auth | Açıklama |
|---|---|---|---|
| GET | `/api/payments` | ✅ | Tüm ödemeler |
| GET | `/api/payments/{id}` | ✅ | Ödeme detayı |
| POST | `/api/payments/process` | ✅ | Standart ödeme işle |
| POST | `/api/payments/{id}/refund` | ✅ | İade |
| POST | `/api/payments/gateway/initiate` | ✅ | 3D Secure başlat → HTML markup |
| POST | `/api/payments/{id}/callback` | ❌ | Banka callback işle (Pos.Web çağırır) |

---

## 🖥️ Frontend (Pos.Web)

Pos.Web, API ile iletişim kuran bir **ASP.NET Core MVC** uygulamasıdır. JWT token'ı Session'da saklar ve her istekte `Authorization: Bearer` header'ı ekler.

### Sayfalar

| URL | Sayfa | Açıklama |
|---|---|---|
| `/` | Dashboard | Genel bakış |
| `/Account/Login` | Giriş | JWT token alır |
| `/Account/Register` | Kayıt | Kullanıcı oluştur |
| `/Products` | Ürün Listesi | Katalog gösterimi |
| `/Customers` | Müşteri Listesi | Müşteri yönetimi |
| `/Orders` | Sipariş Listesi | Tüm siparişler |
| `/Orders/Create` | Sipariş Oluştur | Ürün seç, müşteri ata |
| `/Payments` | Ödeme Geçmişi | Tüm ödeme kayıtları |
| `/Payments/Process?orderId=` | Ödeme Seç | Standart veya 3D Secure |
| `/Payments/GatewayPay?orderId=` | 3D Secure Kart Formu | Kart bilgisi + canlı önizleme |
| `/Payments/GatewayRedirect` | Bankaya Yönlendirme | Auto-submit + spinner |
| `/Payments/GatewayCallback/{id}` | Banka Callback | Ödeme sonucu işle |
| `/Payments/CallbackSuccess` | Başarı Sonucu | Onay sayfası |
| `/Payments/CallbackFail` | Hata Sonucu | Hata + tekrar deneme |

### ApiClient Yapısı

```csharp
// Tüm HTTP isteklerini saran servis
public class ApiClient
{
    // JWT token otomatik eklenir
    private void SetAuthHeader() { ... }

    // Tip-güvenli generic metotlar
    private Task<T?> GetAsync<T>(string url) { ... }
    private Task<(bool, T?, string?)> PostAsync<T>(string url, object body) { ... }

    // Domain metodları
    public Task<List<PaymentViewModel>?> GetPaymentsAsync();
    public Task<(bool, GatewayMarkupViewModel?, string?)> InitiateGatewayPaymentAsync(vm, callbackBaseUrl);
    public Task<(bool, string?)> ProcessGatewayCallbackAsync(paymentId, parameters);
}
```

---

## 🔐 Güvenlik

### JWT Authentication

```json
"JwtSettings": {
  "Secret": "...",         // Min 32 karakter
  "Issuer": "PosApi",
  "Audience": "PosClients",
  "ExpiresInMinutes": 60
}
```

### CORS Politikası

```csharp
// Geliştirme: Tüm origin'lere izin
policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()

// Prodüksiyon: Sadece Pos.Web adresi izinlenmelidir
```

### 3D Secure Güvenlik Katmanları

1. **HTTPS zorunluluğu** — Kart verisi hiçbir zaman HTTP üzerinden gitmez
2. **SHA-512 Hash doğrulama** — Banka yanıtı manipüle edilmiş mi?
3. **mdStatus kontrol** — 0-9 arası durum kodları tek tek doğrulanır
4. **`[IgnoreAntiforgeryToken]`** — Banka POST'u kendi form'unu gönderir
5. **`[AllowAnonymous]`** — Callback endpoint'i token gerektirmez (banka çağırır)
6. **Kart verisi saklanmaz** — Kart numarası hiçbir zaman veritabanına yazılmaz

---

## ⚙️ Yapılandırma

### `Pos.EndPoint/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=pos.db"
  },
  "JwtSettings": {
    "Secret": "PosSystemSuperSecretKey_2026_MinLength32Chars!",
    "Issuer": "PosApi",
    "Audience": "PosClients",
    "ExpiresInMinutes": "60"
  },
  "GarantiBank": {
    "Mode": "TEST",
    "Url3DGate": "https://sanalposprovtest.garantibbva.com.tr/servlet/gt3dengine",
    "ProvisionPassword": "*** GERÇEK DEĞER ***",
    "StoreKey": "*** GERÇEK DEĞER ***",
    "MerchantId": "*** GERÇEK DEĞER ***",
    "TerminalId": "*** GERÇEK DEĞER ***",
    "TerminalUserId": "PROVAUT",
    "ApiVersion": "v0.01",
    "SecurityLevel": "3D_PAY",
    "SkipHashVerification": false,
    "FailUrl":    "https://[web-host]/Payments/GatewayCallback/:paymentId",
    "SuccessUrl": "https://[web-host]/Payments/GatewayCallback/:paymentId"
  }
}
```

### `Pos.EndPoint/appsettings.Development.json`

```json
{
  "GarantiBank": {
    "Mode": "TEST",
    "Url3DGate": "https://sanalposprovtest.garantibbva.com.tr/servlet/gt3dengine",
    "SkipHashVerification": true,
    "FailUrl":    "https://localhost:[WEB_PORT]/Payments/GatewayCallback/:paymentId",
    "SuccessUrl": "https://localhost:[WEB_PORT]/Payments/GatewayCallback/:paymentId"
  }
}
```

> ⚠️ `[WEB_PORT]` = Pos.Web'in çalıştığı port. Dinamik URL özelliği sayesinde bu değer artık sadece fallback olarak kullanılır.

### `Pos.Web/appsettings.json`

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:[API_PORT]"
  }
}
```

---

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler

- .NET 9.0 SDK
- Garanti Bankası test terminal bilgileri (isteğe bağlı)

### 1. Repository'yi Klonla

```bash
git clone <repo-url>
cd Pos
```

### 2. Yapılandırmayı Güncelle

`Pos.EndPoint/appsettings.Development.json` dosyasına Garanti Bankası test bilgilerini gir:

```bash
# Garanti BBVA sanal pos test panelinden alınan bilgiler
ProvisionPassword, StoreKey, MerchantId, TerminalId, TerminalUserId
```

### 3. API'yi Başlat

```bash
cd Pos.EndPoint
dotnet run
# → https://localhost:7500
# → Swagger: https://localhost:7500/swagger
```

Uygulama başlatılırken migration'lar **otomatik** çalışır, veritabanı oluşturulur.

### 4. Web Arayüzünü Başlat

```bash
cd Pos.Web
dotnet run
# → https://localhost:7600
```

`Pos.Web/appsettings.json` içindeki `ApiSettings:BaseUrl`'i API'nin gerçek portuna göre ayarla.

### 5. Test Verisi Yükle (İsteğe Bağlı)

```bash
chmod +x seed-data.sh
./seed-data.sh
```

### 6. Tüm Sistemi Derle

```bash
dotnet build Pos.sln
# 0 Hata, 0 Uyarı
```

---

## 🗄️ Veritabanı

- **Motor**: SQLite (geliştirme) → Production'da PostgreSQL/SQL Server geçişi mümkün
- **Migration**: Her modülün kendi `Migrations/` klasörü
- **Schema izolasyonu**: Her modül kendi DB şemasını kullanır

| Modül | Schema | Tablolar |
|---|---|---|
| Identity | `identity` | AspNetUsers, AspNetRoles... |
| Catalog | `catalog` | Categories, Products |
| Customers | `customers` | Customers |
| Orders | `orders` | Orders, OrderItems |
| Payments | `payments` | Payments |

```bash
# Migration oluşturma
dotnet ef migrations add InitialCreate \
  --project Modules/Payments/Pos.Modules.Payments.Infrastructure \
  --startup-project Pos.EndPoint

# Migration uygulama
dotnet ef database update \
  --project Modules/Payments/Pos.Modules.Payments.Infrastructure \
  --startup-project Pos.EndPoint
```

---

## 🛠️ Teknoloji Yığını

| Katman | Teknoloji | Versiyon |
|---|---|---|
| Backend | ASP.NET Core Web API | 9.0 |
| Frontend | ASP.NET Core MVC + Razor | 9.0 |
| ORM | Entity Framework Core | 9.0.4 |
| Veritabanı | SQLite | — |
| CQRS | MediatR | 12.4.1 |
| Validation | FluentValidation | 11.11.0 |
| Authentication | JWT Bearer | 9.0.4 |
| API Docs | Swagger / Swashbuckle | 7.3.1 |
| UI Framework | Bootstrap | 5.x |
| Icons | Bootstrap Icons | 1.11.3 |
| Encoding | System.Text.Encoding.CodePages | 9.0.4 (ISO-8859-9) |
| Options | Microsoft.Extensions.Options | 9.0.4 |
| Hash | SHA1 / SHA512 | BCL |

---

## 🔧 Geliştirme Prensipleri

| Prensip | Uygulama |
|---|---|
| **SOLID** | Her sınıfın tek sorumluluğu, interface'lere bağımlılık |
| **DRY** | Shared Kernel ile tekrar ortadan kaldırıldı |
| **Separation of Concerns** | 4 katmanlı modül yapısı |
| **Dependency Inversion** | Tüm bağımlılıklar interface üzerinden |
| **Result Pattern** | `Result<T>` / `OperationResult<T>` ile exception-free akış |
| **Clean Architecture** | Domain dışarıya bağımlı değil |
| **CQRS** | Okuma ve yazma operasyonları ayrıştırıldı |
| **Repository Pattern** | Veri erişimi soyutlandı |

---

## 📈 Ölçeklenebilirlik

Bu mimari aşağıdaki yönlerde kolayca genişletilebilir:

### Yeni Ödeme Sağlayıcısı Eklemek

```
1. Pos.Providers.YeniBank/ projesi oluştur
2. IGarantiBankPaymentProvider yerine IGatewayPaymentService implement et
3. GatewayPaymentService içinde sağlayıcıyı seç (strategy pattern)
4. appsettings'e yeni sağlayıcı ayarları ekle
```

### Microservice'e Geçiş

```
Modüller → Her modül ayrı deployment unit
Veritabanı → Her modülün kendi DB server'ı
İletişim → HTTP/gRPC yerine mesaj kuyruğu (RabbitMQ/Kafka)
```

### Event-Driven Mimari

```
Domain Events → Integration Events → Message Bus
OrderCreated → PaymentModule abone olur → Otomatik ödeme başlatılır
PaymentCompleted → OrderModule abone olur → Sipariş tamamlanır
```

---

## 📊 Proje İstatistikleri

| Metrik | Değer |
|---|---|
| Toplam Proje | 22 |
| Modül Sayısı | 5 (Identity, Catalog, Customers, Orders, Payments) |
| Payment Provider | 1 (Garanti BBVA 3D Secure) |
| API Endpoint | ~25 |
| Web Sayfası | ~20 |
| Target Framework | net9.0 |

---

## 🔗 Bağımlılık Haritası

```
Pos.EndPoint
  ├── Pos.Shared
  ├── Identity.Infrastructure → Identity.Application → Identity.Domain → Pos.Shared
  ├── Catalog.Infrastructure  → Catalog.Application  → Catalog.Domain  → Pos.Shared
  ├── Customers.Infrastructure→ Customers.Application→ Customers.Domain→ Pos.Shared
  ├── Orders.Infrastructure   → Orders.Application   → Orders.Domain   → Pos.Shared
  │       └── Catalog.Infrastructure (cross-module)
  │       └── Customers.Infrastructure (cross-module)
  └── Payments.Infrastructure → Payments.Application → Payments.Domain → Pos.Shared
          └── Orders.Infrastructure (cross-module)
          └── Pos.Providers.GarantiBank

Pos.Web
  └── ApiClient (HTTP → Pos.EndPoint)
```

---

*Son güncelleme: Nisan 2026*

