# Proje Yapısı ve Özellikler

## 🎯 Proje Özeti

Bu proje **Modular Monolithic Architecture** ile geliştirilmiş bir **POS (Point of Sale) Ödeme Sistemi**dir.

## 📂 Klasör Yapısı

```
Pos/
│
├── 📁 Pos.Shared/                          # Shared Kernel
│   ├── Domain/
│   │   ├── BaseEntity.cs                   # Base entity sınıfı
│   │   └── IDomainEvent.cs                 # Domain event interface
│   ├── Abstractions/
│   │   └── IRepository.cs                  # Generic repository interface
│   └── Common/
│       └── Result.cs                       # Result pattern
│
├── 📁 Pos.EndPoint/                        # Backend API Host
│   ├── Program.cs                          # Startup, DI, JWT, Swagger
│   ├── appsettings.json                    # Connection string, JWT settings
│   └── pos.db                              # SQLite database (runtime)
│
├── 📁 Pos.Web/                             # Frontend MVC
│   ├── Controllers/                        # Account, Products, Customers, Orders, Payments
│   ├── Views/                              # Razor views
│   ├── Models/                             # ViewModels
│   ├── Services/                           # ApiClient (HTTP wrapper)
│   └── Program.cs                          # HttpClient, Session
│
└── 📁 Modules/                             # Bağımsız Modüller
    │
    ├── 📁 Identity/                        # Kimlik Doğrulama Modülü
    │   ├── Domain/
    │   │   └── Entities/AppUser.cs         # IdentityUser<Guid> türevi
    │   ├── Application/
    │   │   ├── Commands/                   # Register, Login
    │   │   ├── DTOs/                       # Data transfer objects
    │   │   ├── Interfaces/IJwtTokenService.cs
    │   │   └── Validators/                 # FluentValidation
    │   ├── Infrastructure/
    │   │   ├── Persistence/IdentityDbContext.cs
    │   │   ├── Services/JwtTokenService.cs # JWT token üretimi
    │   │   └── Extensions/                 # DI registration
    │   └── Presentation/
    │       └── Controllers/AuthController.cs
    │
    ├── 📁 Catalog/                         # Ürün Kataloğu Modülü
    │   ├── Domain/
    │   │   └── Entities/
    │   │       ├── Category.cs             # Kategori entity
    │   │       └── Product.cs              # Ürün entity + business rules
    │   ├── Application/
    │   │   ├── Commands/                   # Create, Update, Delete
    │   │   ├── Queries/                    # GetAll, GetById
    │   │   ├── Handlers/                   # CQRS handlers
    │   │   └── Interfaces/                 # Repository interfaces
    │   ├── Infrastructure/
    │   │   ├── Persistence/CatalogDbContext.cs
    │   │   └── Repositories/               # EF implementations
    │   └── Presentation/
    │       └── Controllers/
    │           ├── CategoriesController.cs
    │           └── ProductsController.cs
    │
    ├── 📁 Customers/                       # Müşteri Yönetimi Modülü
    │   ├── Domain/Entities/Customer.cs
    │   ├── Application/                    # Commands, Queries, Handlers
    │   ├── Infrastructure/                 # CustomersDbContext, Repository
    │   └── Presentation/Controllers/CustomersController.cs
    │
    ├── 📁 Orders/                          # Sipariş Yönetimi Modülü
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   ├── Order.cs                # Aggregate root
    │   │   │   └── OrderItem.cs            # Sipariş kalemi
    │   │   └── Enums/OrderStatus.cs
    │   ├── Application/
    │   │   ├── Commands/                   # CreateOrder, UpdateStatus, Cancel
    │   │   ├── Queries/                    # GetAll, GetById
    │   │   └── Interfaces/                 # IProductService, ICustomerService
    │   ├── Infrastructure/
    │   │   ├── Persistence/OrdersDbContext.cs
    │   │   ├── Repositories/OrderRepository.cs
    │   │   └── Services/                   # Cross-module services
    │   │       ├── ProductService.cs       # Catalog'dan ürün bilgisi al
    │   │       └── CustomerService.cs      # Customers'dan müşteri bilgisi al
    │   └── Presentation/Controllers/OrdersController.cs
    │
    └── 📁 Payments/                        # Ödeme Modülü
        ├── Domain/
        │   ├── Entities/Payment.cs         # Ödeme kaydı + business logic
        │   └── Enums/                      # PaymentMethod, PaymentStatus
        ├── Application/
        │   ├── Commands/                   # ProcessPayment, Refund
        │   ├── Queries/                    # GetAll, GetById
        │   └── Interfaces/IOrderStatusService.cs
        ├── Infrastructure/
        │   ├── Persistence/PaymentsDbContext.cs
        │   ├── Repositories/PaymentRepository.cs
        │   └── Services/OrderStatusService.cs  # Orders'a sipariş durumu güncelle
        └── Presentation/Controllers/PaymentsController.cs
```

## 🔗 Bağımlılık Grafiği

```
┌──────────────────────────────────────────────────────────────┐
│                      Pos.EndPoint (API Host)                  │
│  - Tüm modüllerin Infrastructure ve Presentation'ını ref eder│
└────────────┬──────────────────────────────────────┬──────────┘
             │                                      │
    ┌────────▼────────┐                   ┌────────▼────────┐
    │  Pos.Shared     │                   │  All Modules    │
    │  (Kernel)       │◄──────────────────┤  Infrastructure │
    └─────────────────┘                   └─────────────────┘
             ▲                                      │
             │                                      │
        ┌────┴────────────────────────────┐        │
        │  All Module Domains             │        │
        │  (BaseEntity, Result kullanır)  │        │
        └─────────────────────────────────┘        │
                                                    │
                                           ┌────────▼────────┐
                                           │  Presentations  │
                                           │  (Controllers)  │
                                           └─────────────────┘
```

## 🔄 Veri Akışı

### Sipariş Oluşturma İş Akışı

```
1. OrdersController (Presentation)
   ↓
2. CreateOrderCommandHandler (Application)
   ↓
3. ICustomerService.GetCustomerNameAsync() → CustomersDbContext
   ↓
4. IProductService.GetProductAsync() → CatalogDbContext
   ↓
5. Order.Create() + Order.AddItem() (Domain Logic)
   ↓
6. IProductService.DecreaseStockAsync() → CatalogDbContext
   ↓
7. IOrderRepository.AddAsync() → OrdersDbContext
   ↓
8. Result<OrderDto> döner
```

### Ödeme İş Akışı

```
1. PaymentsController (Presentation)
   ↓
2. ProcessPaymentCommandHandler (Application)
   ↓
3. IOrderStatusService.GetOrderInfoAsync() → OrdersDbContext
   ↓
4. Payment.Create() + Payment.Complete() (Domain)
   ↓
5. IOrderStatusService.MarkOrderAsCompletedAsync() → OrdersDbContext
   ↓
6. IPaymentRepository.AddAsync() → PaymentsDbContext
   ↓
7. Result<PaymentDto> döner
```

## 🎨 Frontend-Backend İletişimi

```
┌────────────┐         HTTPS/JSON          ┌────────────┐
│            │   ◄──── ApiClient ─────►    │            │
│  Pos.Web   │   (HttpClient + Session)    │ Pos.EndPoint│
│  (MVC)     │   JWT Bearer Token Header   │  (API)     │
│            │                              │            │
└────────────┘                              └────────────┘
     │                                            │
     │ Views + ViewModels                         │ DTOs + Controllers
     │ Session (JWT Storage)                      │ CQRS + MediatR
     │                                            │
     └─── Razor Pages ─────────────────── RESTful Endpoints ───┘
```

## 🧩 Katman Sorumlulukları

| Katman | Sorumluluk | Bağımlılık |
|--------|-----------|-----------|
| **Domain** | Entity'ler, Business Rules, Enums | Sadece Shared |
| **Application** | CQRS, Validation, DTOs, Interfaces | Domain + Shared |
| **Infrastructure** | DbContext, Repositories, External Services | Application |
| **Presentation** | API Controllers, HTTP Layer | Application |

## 🔐 Güvenlik Katmanları

1. **JWT Authentication**: Tüm hassas endpoint'ler token gerektirir
2. **Password Hashing**: ASP.NET Core Identity ile güvenli hash
3. **HTTPS**: Prod ortamda zorunlu
4. **CORS**: Sadece belirtilen origin'lere izin
5. **Validation**: FluentValidation ile input doğrulama

## ⚡ Performans Özellikleri

- **Eager Loading**: Navigation property'ler Include() ile yüklenir
- **Async/Await**: Tüm I/O işlemleri asenkron
- **Connection Pooling**: EF Core otomatik yönetir
- **Caching**: Hazır - gerektiğinde `IDistributedCache` eklenebilir

## 🧪 Test Stratejisi

Her modül bağımsız test edilebilir:

```bash
# Unit Tests (eklenebilir)
- Domain.Tests: Business logic testleri
- Application.Tests: Handler testleri (mocked repositories)

# Integration Tests (eklenebilir)
- Infrastructure.Tests: Repository testleri (in-memory DB)
- Presentation.Tests: Controller testleri (TestServer)
```

## 📈 Ölçeklenebilirlik

Bu mimari ileride şu yönlerde ölçeklenebilir:

1. **Microservice'e Dönüşüm**: Her modül ayrı servise çevrilebilir
2. **Event-Driven**: Domain events ile modüller arası mesajlaşma
3. **CQRS Read/Write DB**: Read model için ayrı veritabanı
4. **API Gateway**: Birden fazla API instance için gateway eklenebilir

## 🔧 Geliştirme Prensipleri

✅ **SOLID Principles**  
✅ **DRY (Don't Repeat Yourself)**  
✅ **Separation of Concerns**  
✅ **Dependency Inversion**  
✅ **Interface Segregation**  

Her modül kendi sorumluluk alanında bağımsız çalışır ve diğer modüllerle sadece interface'ler üzerinden iletişim kurar.

