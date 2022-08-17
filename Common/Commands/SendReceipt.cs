using Common.DTO;

namespace Common.Commands;

public class SendReceipt : IOrderMessage
{
    public Guid CorrelationId { get; set; }
    public Order Order { get; set; }
}