using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Card;

public class CreateCardInCasConsumer : IConsumer<CreateCardInCas>
{
    public async Task Consume(ConsumeContext<CreateCardInCas> context)
    {
        string cardNumber;
        
        if (string.IsNullOrEmpty(context.Message.GiftcardItem.CardNumber))
        {
            Log.Information("Consumer: Create Card in CAS, Order ID: {OrderId}", context.Message.GiftcardItem.OrderId);
            Random rnd = new Random();
            cardNumber = rnd.Next(100000, 999999).ToString();

        }
        else
        {
            Log.Information("Consumer (skipped): Create Card in CAS");
            cardNumber = context.Message.GiftcardItem.CardNumber;
        }

        await context.Publish<ICardCreated>(new
        {
            CorrelationId = context.Message.CorrelationId,
            GiftcardItem = context.Message.GiftcardItem with { CardNumber = cardNumber }
        });
    }
}