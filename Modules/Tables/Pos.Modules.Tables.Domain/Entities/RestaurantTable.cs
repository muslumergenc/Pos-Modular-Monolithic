using Pos.Shared.Domain;

namespace Pos.Modules.Tables.Domain.Entities;

public enum TableStatus
{
    Available = 0,   // Boş
    Occupied  = 1,   // Dolu
    Reserved  = 2    // Rezerve
}

public class RestaurantTable : BaseEntity
{
    public int Number       { get; private set; }
    public string? Label    { get; private set; }   // Örn: "Teras-3"
    public int Capacity     { get; private set; }
    public TableStatus Status { get; private set; } = TableStatus.Available;

    private RestaurantTable() { }

    public static RestaurantTable Create(int number, int capacity, string? label = null)
    {
        if (capacity <= 0) throw new ArgumentException("Kapasite en az 1 olmalıdır.");
        return new RestaurantTable { Number = number, Capacity = capacity, Label = label };
    }

    public void Update(int number, int capacity, string? label)
    {
        Number   = number;
        Capacity = capacity;
        Label    = label;
        SetUpdatedAt();
    }

    public void SetStatus(TableStatus status)
    {
        Status = status;
        SetUpdatedAt();
    }

    public void Open()  => SetStatus(TableStatus.Available);
    public void Close() => SetStatus(TableStatus.Occupied);
}
