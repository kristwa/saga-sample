namespace Common.DTO;

public record Order
{
    public Guid OrderId { get; set; }
    public OrderType OrderType { get; set; } = OrderType.Purchase;
    public string? CardNumber { get; set; }
}

public enum OrderType
{
    Deposit, Purchase
}