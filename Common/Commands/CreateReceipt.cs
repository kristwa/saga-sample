using Common.DTO;

namespace Common.Commands;

public class CreateReceipt : IOrderMessage
{
    public Guid CorrelationId { get; set; }
    public Order Order { get; set; }
}