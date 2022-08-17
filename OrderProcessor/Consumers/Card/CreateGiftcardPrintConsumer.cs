using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Card;

public class CreateGiftcardPrintConsumer : IConsumer<CreateGiftcardPrint>
{
    public async Task Consume(ConsumeContext<CreateGiftcardPrint> context)
    {
        if (context.Message.GiftcardItem.HasPrintDesign)
        {
            Log.Information("Consumer: Create Giftcard Print, Card: {CardNo}", context.Message.GiftcardItem.CardNumber);
        }
        else
        {
            Log.Information("Consumer (Skipped): Create Giftcard Print, card {CardNo}", context.Message.GiftcardItem.CardNumber);
        }
        
        await context.Publish<ICardDesignPrintCreated>(new
        {
            context.Message.CorrelationId,
            context.Message.GiftcardItem
        });
    }
}