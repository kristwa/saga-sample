using Common.DTO;

namespace Common.Commands;

public class CreateCardInCas : IGiftcardItemMessage
{
    public Guid CorrelationId { get; set; }
    public GiftcardItem GiftcardItem { get; set; }
}

public class FinalizeOrder : IGiftcardItemMessage
{
    public Guid CorrelationId { get; set; }
    public GiftcardItem GiftcardItem { get; set; }
}