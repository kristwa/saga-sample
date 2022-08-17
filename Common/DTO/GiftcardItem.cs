namespace Common.DTO;

public record GiftcardItem
{
    public Guid ItemId { get; set; }
    public Guid OrderId { get; set; }
    public string? CardNumber { get; set; }
    public int Amount { get; set; } = 0;
    public OrderType OrderType { get; set; } = OrderType.Purchase;
    public bool HasPrintDesign { get; set; }
}