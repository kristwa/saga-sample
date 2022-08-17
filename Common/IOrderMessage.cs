using Common.DTO;

namespace Common;

public interface IOrderMessage
{
    Guid CorrelationId { get; }
    Order Order { get; }
}