#!/bin/bash

API_URL="http://localhost:5500"
CURL_OPTS="-s"  # -s: silent

echo "🔄 API'yi test ediyorum..."
curl $CURL_OPTS $API_URL/swagger/v1/swagger.json > /dev/null
if [ $? -ne 0 ]; then
  echo "❌ API çalışmıyor! Önce 'cd Pos.EndPoint && dotnet run' komutunu çalıştırın."
  exit 1
fi

echo "✅ API aktif. Test verileri oluşturuluyor..."

# 1. Kullanıcı kaydı
echo "1️⃣  Kullanıcı kaydı..."
REGISTER_RESPONSE=$(curl $CURL_OPTS -X POST "$API_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Admin User",
    "email": "admin@pos.com",
    "password": "Admin123",
    "confirmPassword": "Admin123"
  }')
echo "   ✓ Kullanıcı: admin@pos.com / Admin123"

# 2. Login - JWT token al
echo "2️⃣  Giriş yapılıyor..."
LOGIN_RESPONSE=$(curl $CURL_OPTS -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@pos.com", "password": "Admin123"}')

TOKEN=$(echo $LOGIN_RESPONSE | python3 -c "import sys, json; print(json.load(sys.stdin).get('token', ''))" 2>/dev/null)
if [ -z "$TOKEN" ]; then
  echo "   ❌ Token alınamadı. Muhtemelen kullanıcı zaten kayıtlı."
  # Tekrar dene
  LOGIN_RESPONSE=$(curl $CURL_OPTS -X POST "$API_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d '{"email": "admin@pos.com", "password": "Admin123"}')
  TOKEN=$(echo $LOGIN_RESPONSE | python3 -c "import sys, json; print(json.load(sys.stdin).get('token', ''))" 2>/dev/null)
  if [ -z "$TOKEN" ]; then
    echo "   ❌ Giriş başarısız oldu."
    exit 1
  fi
fi
echo "   ✓ JWT Token alındı"

# 3. Kategoriler
echo "3️⃣  Kategoriler oluşturuluyor..."
CAT1=$(curl $CURL_OPTS -X POST "$API_URL/api/categories" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name": "İçecekler", "description": "Sıcak ve soğuk içecekler"}' \
  | python3 -c "import sys, json; print(json.load(sys.stdin).get('id', ''))" 2>/dev/null)

CAT2=$(curl $CURL_OPTS -X POST "$API_URL/api/categories" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name": "Yiyecekler", "description": "Atıştırmalıklar ve yemekler"}' \
  | python3 -c "import sys, json; print(json.load(sys.stdin).get('id', ''))" 2>/dev/null)

echo "   ✓ 2 kategori oluşturuldu"

# 4. Ürünler
echo "4️⃣  Ürünler oluşturuluyor..."
curl $CURL_OPTS -X POST "$API_URL/api/products" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\": \"Espresso\", \"description\": \"Çift shot espresso\", \"price\": 45.50, \"stock\": 100, \"categoryId\": \"$CAT1\"}" > /dev/null

curl $CURL_OPTS -X POST "$API_URL/api/products" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\": \"Cappuccino\", \"description\": \"Sütlü cappuccino\", \"price\": 55.00, \"stock\": 80, \"categoryId\": \"$CAT1\"}" > /dev/null

curl $CURL_OPTS -X POST "$API_URL/api/products" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\": \"Çikolatalı Kek\", \"description\": \"Ev yapımı kek\", \"price\": 35.00, \"stock\": 50, \"categoryId\": \"$CAT2\"}" > /dev/null

echo "   ✓ 3 ürün oluşturuldu"

# 5. Müşteriler
echo "5️⃣  Müşteriler oluşturuluyor..."
CUST1=$(curl $CURL_OPTS -X POST "$API_URL/api/customers" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"fullName": "Ahmet Yılmaz", "email": "ahmet@mail.com", "phone": "05551234567", "address": "İstanbul Kadıköy"}' \
  | python3 -c "import sys, json; print(json.load(sys.stdin).get('id', ''))" 2>/dev/null)

CUST2=$(curl $CURL_OPTS -X POST "$API_URL/api/customers" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"fullName": "Ayşe Kaya", "email": "ayse@mail.com", "phone": "05559876543", "address": "Ankara Çankaya"}' \
  | python3 -c "import sys, json; print(json.load(sys.stdin).get('id', ''))" 2>/dev/null)

echo "   ✓ 2 müşteri oluşturuldu"

echo ""
echo "✅ Test verileri başarıyla oluşturuldu!"
echo ""
echo "📝 Giriş Bilgileri:"
echo "   Email: admin@pos.com"
echo "   Şifre: Admin123"
echo ""
echo "🌐 URL'ler:"
echo "   API Swagger: http://localhost:5500/swagger"
echo "   MVC Frontend: http://localhost:5600"
echo ""
