using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Card;

public class DepositCardConsumer : IConsumer<DepositCard>
{
    public async Task Consume(ConsumeContext<DepositCard> context)
    {
        Log.Information("Consumer: Deposit card, card no: {CardNumber}", context.Message.GiftcardItem.CardNumber);

        await context.Publish<ICardDeposited>(new
            { CorrelationId = context.Message.CorrelationId, GiftcardItem = context.Message.GiftcardItem });
    }
}