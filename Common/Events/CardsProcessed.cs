using Common.DTO;

namespace Common.Events;

public class CardsProcessed : IOrderMessage
{
    public Guid CorrelationId { get; set; }
    public Order Order { get; set; }
}