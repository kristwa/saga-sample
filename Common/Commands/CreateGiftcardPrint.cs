using Common.DTO;

namespace Common.Commands;

public class CreateGiftcardPrint : IGiftcardItemMessage
{
    public Guid CorrelationId { get; set; }
    public GiftcardItem GiftcardItem { get; set; }
}