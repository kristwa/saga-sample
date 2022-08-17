using Common.DTO;

namespace Common.Events;

public class ProcessGiftcardItemRequestCreated : IGiftcardItemMessage
{
    public Guid CorrelationId { get; set; }
    public GiftcardItem GiftcardItem { get; set; }
}