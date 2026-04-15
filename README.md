# 🛒 POS Ödeme Sistemi

**Modular Monolithic Architecture** ile geliştirilmiş, .NET 9.0 tabanlı POS (Point of Sale) ödeme sistemi.

## 📐 Mimari

Proje **Modular Monolithic** mimari prensiplerine göre tasarlanmıştır:

```
Pos/
├── Pos.EndPoint/          # Backend API (Host)
├── Pos.Web/               # Frontend MVC
├── Pos.Shared/            # Shared Kernel (Domain primitives)
└── Modules/               # Bağımsız Modüller
    ├── Identity/          # Kimlik doğrulama ve JWT
    ├── Catalog/           # Ürün ve kategori yönetimi
    ├── Customers/         # Müşteri yönetimi
    ├── Orders/            # Sipariş yönetimi
    └── Payments/          # Ödeme işlemleri
```

### Her Modül 4 Katmandan Oluşur:

- **Domain**: Entity'ler, Value Objects, Domain Events
- **Application**: CQRS (MediatR), DTOs, Validators (FluentValidation)
- **Infrastructure**: DbContext, Repository'ler, External Services
- **Presentation**: API Controllers

## 🛠️ Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| **Backend** | ASP.NET Core 9.0 Web API |
| **Frontend** | ASP.NET Core 9.0 MVC |
| **Database** | SQLite + Entity Framework Core 9 |
| **Authentication** | JWT Bearer Token |
| **CQRS** | MediatR 12.4 |
| **Validation** | FluentValidation 11.11 |
| **API Documentation** | Swagger/OpenAPI |
| **Architecture** | Clean Architecture + Modular Monolith |

## 🚀 Kurulum ve Çalıştırma

### 1️⃣ API Backend'i Başlat

```bash
cd Pos.EndPoint
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

API şu adreslerde yayına girecektir:
- **HTTPS**: https://localhost:7001
- **HTTP**: http://localhost:5001
- **Swagger UI**: https://localhost:7001/swagger

### 2️⃣ MVC Frontend'i Başlat

```bash
cd Pos.Web
dotnet run --urls "https://localhost:7002;http://localhost:5002"
```

MVC şu adreslerde yayına girecektir:
- **HTTPS**: https://localhost:7002
- **HTTP**: http://localhost:5002

## 📚 API Endpoint'leri

### 🔐 **Identity Module** (`/api/auth`)

| Endpoint | Method | Açıklama |
|----------|--------|----------|
| `/api/auth/register` | POST | Yeni kullanıcı kaydı |
| `/api/auth/login` | POST | Login - JWT token döner |

### 📦 **Catalog Module**

| Endpoint | Method | Auth | Açıklama |
|----------|--------|------|----------|
| `/api/categories` | GET | ❌ | Tüm kategoriler |
| `/api/categories/{id}` | GET | ❌ | Kategori detayı |
| `/api/categories` | POST | ✅ | Yeni kategori |
| `/api/categories/{id}` | PUT | ✅ | Kategori güncelle |
| `/api/categories/{id}` | DELETE | ✅ | Kategori sil |
| `/api/products` | GET | ❌ | Tüm ürünler |
| `/api/products/{id}` | GET | ❌ | Ürün detayı |
| `/api/products` | POST | ✅ | Yeni ürün |
| `/api/products/{id}` | PUT | ✅ | Ürün güncelle |
| `/api/products/{id}` | DELETE | ✅ | Ürün sil |

### 👥 **Customers Module** (`/api/customers`)

| Endpoint | Method | Auth | Açıklama |
|----------|--------|------|----------|
| `/api/customers` | GET | ✅ | Tüm müşteriler |
| `/api/customers/{id}` | GET | ✅ | Müşteri detayı |
| `/api/customers` | POST | ✅ | Yeni müşteri |
| `/api/customers/{id}` | PUT | ✅ | Müşteri güncelle |
| `/api/customers/{id}` | DELETE | ✅ | Müşteri sil |

### 🛍️ **Orders Module** (`/api/orders`)

| Endpoint | Method | Auth | Açıklama |
|----------|--------|------|----------|
| `/api/orders` | GET | ✅ | Tüm siparişler |
| `/api/orders/{id}` | GET | ✅ | Sipariş detayı |
| `/api/orders` | POST | ✅ | Yeni sipariş |
| `/api/orders/{id}/status` | PATCH | ✅ | Sipariş durumu güncelle |
| `/api/orders/{id}/cancel` | POST | ✅ | Siparişi iptal et |

### 💳 **Payments Module** (`/api/payments`)

| Endpoint | Method | Auth | Açıklama |
|----------|--------|------|----------|
| `/api/payments` | GET | ✅ | Tüm ödemeler |
| `/api/payments/{id}` | GET | ✅ | Ödeme detayı |
| `/api/payments/process` | POST | ✅ | Ödeme işle |
| `/api/payments/{id}/refund` | POST | ✅ | İade işlemi |

## 🎯 Kullanım Senaryosu

### 1. Kullanıcı Kaydı ve Giriş
```bash
# Kayıt
POST /api/auth/register
{
  "fullName": "Test User",
  "email": "test@example.com",
  "password": "Test123",
  "confirmPassword": "Test123"
}

# Giriş
POST /api/auth/login
{
  "email": "test@example.com",
  "password": "Test123"
}
# Response: { "token": "eyJ...", "expiresAt": "..." }
```

### 2. Kategori ve Ürün Ekle
```bash
# Kategori
POST /api/categories
Authorization: Bearer {token}
{
  "name": "İçecekler",
  "description": "Soğuk ve sıcak içecekler"
}

# Ürün
POST /api/products
Authorization: Bearer {token}
{
  "name": "Kahve",
  "description": "Espresso kahve",
  "price": 45.50,
  "stock": 100,
  "categoryId": "{categoryId}"
}
```

### 3. Müşteri Ekle
```bash
POST /api/customers
Authorization: Bearer {token}
{
  "fullName": "Ahmet Yılmaz",
  "email": "ahmet@mail.com",
  "phone": "05551234567",
  "address": "İstanbul"
}
```

### 4. Sipariş Oluştur
```bash
POST /api/orders
Authorization: Bearer {token}
{
  "customerId": "{customerId}",
  "notes": "Acele teslimat",
  "items": [
    { "productId": "{productId}", "quantity": 2 },
    { "productId": "{productId2}", "quantity": 1 }
  ]
}
```

### 5. Ödeme Al
```bash
POST /api/payments/process
Authorization: Bearer {token}
{
  "orderId": "{orderId}",
  "amount": 136.50,
  "method": 1,  // 0: Cash, 1: CreditCard, 2: DebitCard, 3: Digital
  "notes": "Nakit ödeme"
}
```

## 🗄️ Veritabanı

Proje **SQLite** kullanır. Migration'lar uygulama başlatıldığında otomatik çalışır.

### Manuel Migration Çalıştırma

```bash
# Her modül için ayrı migration oluşturulmuştur
dotnet ef database update --project Modules/Identity/Pos.Modules.Identity.Infrastructure --startup-project Pos.EndPoint
dotnet ef database update --project Modules/Catalog/Pos.Modules.Catalog.Infrastructure --startup-project Pos.EndPoint
dotnet ef database update --project Modules/Customers/Pos.Modules.Customers.Infrastructure --startup-project Pos.EndPoint
dotnet ef database update --project Modules/Orders/Pos.Modules.Orders.Infrastructure --startup-project Pos.EndPoint
dotnet ef database update --project Modules/Payments/Pos.Modules.Payments.Infrastructure --startup-project Pos.EndPoint
```

## 🔑 Güvenlik

- **JWT Token**: 60 dakika geçerlilik
- **Password Requirements**: Minimum 6 karakter, en az 1 rakam
- **Authorization**: Hassas endpoint'ler `[Authorize]` attribute ile korunur

## 📊 Modül İletişimi

Modüller arası iletişim **sadece Infrastructure katmanında** gerçekleşir:

- **Orders** → **Catalog**: Ürün bilgilerini almak ve stok düşürmek için
- **Orders** → **Customers**: Müşteri adını almak için
- **Payments** → **Orders**: Sipariş bilgilerini almak ve durumu güncellemek için

Bu yapı modüler izolasyonu korurken cross-cutting concern'leri yönetir.

## 🎨 Frontend (Pos.Web)

MVC frontend şu sayfaları içerir:

- 🏠 **Ana Sayfa**: Dashboard
- 🔐 **Giriş/Kayıt**: JWT token alır ve Session'da saklar
- 📦 **Ürünler**: Listeleme, oluşturma, detay
- 👥 **Müşteriler**: Listeleme, oluşturma, detay
- 🛍️ **Siparişler**: Listeleme, oluşturma, detay, iptal, ödeme yönlendirme
- 💳 **Ödemeler**: Geçmiş, ödeme işleme, iade

## 📝 Geliştirme Notları

### Build
```bash
dotnet build Pos.sln
```

### Test
```bash
# API'yi test et
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test","email":"test@test.com","password":"Test123","confirmPassword":"Test123"}'
```

### Clean Build
```bash
dotnet clean
rm -f pos.db pos.db-shm pos.db-wal
dotnet build
```

## 🌟 Özellikler

✅ **Modular Monolith** - Her modül bağımsız geliştirilebilir  
✅ **Clean Architecture** - Katmanlar arası bağımlılık kontrolü  
✅ **CQRS Pattern** - MediatR ile command/query ayrımı  
✅ **Repository Pattern** - Veri erişim katmanı soyutlama  
✅ **JWT Authentication** - Stateless güvenlik  
✅ **API Documentation** - Swagger/OpenAPI  
✅ **Responsive UI** - Bootstrap 5 ile modern tasarım  
✅ **Session Management** - Frontend state yönetimi  
✅ **Error Handling** - Result pattern ile type-safe hata yönetimi  

## 📦 Solution Yapısı

```
Pos.sln
├── Pos.Shared (Shared Kernel)
├── Pos.EndPoint (API Host)
├── Pos.Web (MVC Frontend)
└── Modules/
    ├── Identity.Domain
    ├── Identity.Application
    ├── Identity.Infrastructure
    ├── Identity.Presentation
    ├── Catalog.* (4 proje)
    ├── Customers.* (4 proje)
    ├── Orders.* (4 proje)
    └── Payments.* (4 proje)
```

**Toplam**: 21 proje (1 Shared + 1 API + 1 Web + 4 Modül × 4 Katman + 1 EndPoint)

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

**Geliştirme Tarihi**: Nisan 2026  
**Framework**: .NET 9.0  
**Mimari**: Modular Monolithic Architecture

