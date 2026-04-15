# 🚀 Hızlı Başlangıç Kılavuzu

## ⚡ Projeyi Çalıştırma (3 Adım)

### 1️⃣ Backend API'yi Başlat

Terminal #1:
```bash
cd Pos.EndPoint
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

✅ API çalıştı mı kontrol edin: https://localhost:7001/swagger

### 2️⃣ Frontend MVC'yi Başlat

Terminal #2:
```bash
cd Pos.Web
dotnet run --urls "https://localhost:7002;http://localhost:5002"
```

✅ MVC çalıştı mı kontrol edin: https://localhost:7002

### 3️⃣ Test Verileri Yükle (Opsiyonel)

Terminal #3:
```bash
./seed-data.sh
```

Bu script şunları oluşturur:
- ✅ 1 Admin kullanıcı (admin@pos.com / Admin123)
- ✅ 2 Kategori (İçecekler, Yiyecekler)
- ✅ 3 Ürün (Espresso, Cappuccino, Çikolatalı Kek)
- ✅ 2 Müşteri

## 🎯 İlk Kullanım

### Web Arayüzü Üzerinden (Önerilen)

1. **Kayıt Ol**: https://localhost:7002/Account/Register
   - Ad Soyad: `Test User`
   - E-posta: `test@pos.com`
   - Şifre: `Test123`

2. **Giriş Yap**: https://localhost:7002/Account/Login
   - E-posta: `test@pos.com`
   - Şifre: `Test123`

3. **Ana Dashboard**: https://localhost:7002
   - Ürünler, Müşteriler, Siparişler ve Ödemeler modüllerine erişin

### API Üzerinden (cURL)

```bash
# 1. Kayıt Ol
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "API User",
    "email": "api@pos.com",
    "password": "Api123456",
    "confirmPassword": "Api123456"
  }'

# 2. Giriş Yap
TOKEN=$(curl -s -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "api@pos.com", "password": "Api123456"}' \
  | grep -o '"token":"[^"]*' | cut -d'"' -f4)

echo "Token: $TOKEN"

# 3. Kategori Ekle
curl -X POST http://localhost:5001/api/categories \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name": "Elektronik", "description": "Teknoloji ürünleri"}'

# 4. Ürünleri Listele
curl -s http://localhost:5001/api/products | json_pp
```

## 🧪 Tam İş Akışı Testi

### Senaryo: Bir sipariş oluştur ve ödeme al

```bash
# Token'ı değişkene kaydet (yukarıdaki adımlardan)

# 1. Müşteri oluştur
CUSTOMER_ID=$(curl -s -X POST http://localhost:5001/api/customers \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"fullName": "Mehmet Demir", "email": "mehmet@mail.com", "phone": "05551112233", "address": "İzmir"}' \
  | grep -o '"id":"[^"]*' | cut -d'"' -f4)

# 2. Ürün ID'lerini al (varsa)
PRODUCTS=$(curl -s http://localhost:5001/api/products)

# 3. Sipariş oluştur (ürün ID'leri gerekli)
ORDER_ID=$(curl -s -X POST http://localhost:5001/api/orders \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"customerId\": \"$CUSTOMER_ID\",
    \"notes\": \"Acele teslimat\",
    \"items\": [
      {\"productId\": \"PRODUCT_ID_1\", \"quantity\": 2},
      {\"productId\": \"PRODUCT_ID_2\", \"quantity\": 1}
    ]
  }" | grep -o '"id":"[^"]*' | cut -d'"' -f4)

# 4. Ödeme al
curl -X POST http://localhost:5001/api/payments/process \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"orderId\": \"$ORDER_ID\",
    \"amount\": 136.50,
    \"method\": 1,
    \"notes\": \"Kredi kartı\"
  }"
```

## 🛠️ Geliştirme Komutları

### Build
```bash
dotnet build Pos.sln
```

### Clean
```bash
dotnet clean Pos.sln
rm -f pos.db pos.db-shm pos.db-wal
```

### Yeni Migration Ekle
```bash
# Identity modülü için örnek
dotnet ef migrations add MigrationName \
  --project Modules/Identity/Pos.Modules.Identity.Infrastructure \
  --startup-project Pos.EndPoint \
  --context IdentityDbContext \
  --output-dir Persistence/Migrations
```

### Migration Geri Al
```bash
dotnet ef migrations remove \
  --project Modules/Identity/Pos.Modules.Identity.Infrastructure \
  --startup-project Pos.EndPoint \
  --context IdentityDbContext
```

## 📋 Checklist - İlk Çalıştırma

- [ ] .NET 9.0 SDK yüklü mü? (`dotnet --version`)
- [ ] `cd Pos.EndPoint && dotnet run` → API çalışıyor mu?
- [ ] `cd Pos.Web && dotnet run` → MVC çalışıyor mu?
- [ ] Swagger açılıyor mu? (https://localhost:7001/swagger)
- [ ] Kayıt olup giriş yapabildiniz mi?
- [ ] Bir kategori ve ürün ekleyebildiniz mi?
- [ ] Bir müşteri oluşturabildiniz mi?
- [ ] Sipariş oluşturabildiniz mi?
- [ ] Ödeme alabildiniz mi?

## 🐛 Sorun Giderme

### Port zaten kullanımda
```bash
# 7001 ve 5001 portları doluysa farklı portlar belirtin
dotnet run --urls "https://localhost:8001;http://localhost:6001"
```

### Migration hatası
```bash
# Veritabanını sıfırla
rm -f pos.db pos.db-shm pos.db-wal
dotnet run  # Otomatik migration çalışacak
```

### JWT token alınamıyor
- `appsettings.json` içinde `JwtSettings:Secret` en az 32 karakter olmalı
- Kontrol edin: "PosSystemSuperSecretKey_2026_MinLength32Chars!"

### CORS hatası
- API'de CORS politikası "AllowAll" olarak ayarlı
- Farklı origin gerekirse `Program.cs` içinde düzenleyin

## 📞 İletişim

Sorularınız için issue açabilirsiniz.

---

**Hazır!** Projeniz kullanıma hazır. İyi geliştirmeler! 🎉

