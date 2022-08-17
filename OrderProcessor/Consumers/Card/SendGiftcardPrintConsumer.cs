using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Card;

public class SendGiftcardPrintConsumer : IConsumer<SendGiftcard>
{
    public async Task Consume(ConsumeContext<SendGiftcard> context)
    {
        if (context.Message.GiftcardItem.HasPrintDesign)
        {
            Log.Information("Consumer: Send giftcard, Card {CardNo}", context.Message.GiftcardItem.CardNumber);
        }
        else
        {
            Log.Information("Consumer (skipped): Send giftcard, Card {CardNo}", context.Message.GiftcardItem.CardNumber);
        }

        await context.Publish<IGiftcardSent>(new
        {
            context.Message.CorrelationId,
            context.Message.GiftcardItem
        });
    }
}