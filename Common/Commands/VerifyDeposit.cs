using Common.DTO;

namespace Common.Commands;

public class VerifyDeposit : IGiftcardItemMessage
{
    public Guid CorrelationId { get; set; }
    public GiftcardItem GiftcardItem { get; set; }
}