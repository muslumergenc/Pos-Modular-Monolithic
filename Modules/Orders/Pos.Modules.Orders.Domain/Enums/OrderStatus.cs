namespace Pos.Modules.Orders.Domain.Enums;

public enum OrderStatus
{
    Received   = 0,   // Sipariş alındı
    InKitchen  = 1,   // Mutfakta hazırlanıyor
    Ready      = 2,   // Servis için hazır
    Served     = 3,   // Servis edildi
    Completed  = 4,   // Ödendi / tamamlandı
    Cancelled  = 5    // İptal edildi
}

