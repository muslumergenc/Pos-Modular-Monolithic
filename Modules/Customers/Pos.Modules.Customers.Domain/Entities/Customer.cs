using Pos.Shared.Domain;

namespace Pos.Modules.Customers.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Customer() { }

    public static Customer Create(string fullName, string email, string phone, string? address = null)
        => new Customer { FullName = fullName, Email = email, Phone = phone, Address = address };

    public void Update(string fullName, string email, string phone, string? address)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        SetUpdatedAt();
    }
}

