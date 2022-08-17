using Common.DTO;

namespace Common;

public interface IGiftcardItemMessage
{
    Guid CorrelationId { get; }
    GiftcardItem GiftcardItem { get; }
}