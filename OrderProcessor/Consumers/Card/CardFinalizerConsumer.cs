using Common.Commands;
using Common.Events;
using MassTransit;

namespace OrderProcessor.Consumers.Card;

public class CardFinalizerConsumer : IConsumer<FinalizeOrder>
{
    public async Task Consume(ConsumeContext<FinalizeOrder> context)
    {
        // lets assume only one card per order, so should always trigger order saga
        
        await context.Publish<CardsProcessed>(new CardsProcessed()
        {
            Order = new()
            {
                OrderId = context.Message.GiftcardItem.OrderId,
                OrderType = context.Message.GiftcardItem.OrderType
            },
            CorrelationId = context.Message.GiftcardItem.OrderId
        });
    }
}