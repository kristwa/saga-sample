using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Card;

public class VerifyDepositConsumer : IConsumer<VerifyDeposit>
{
    public async Task Consume(ConsumeContext<VerifyDeposit> context)
    {
        Log.Information("Consumer: Verify Deposit for card {Card}", context.Message.GiftcardItem.CardNumber);

        await context.Publish<ICardDepositVerified>(new
        {
            context.Message.CorrelationId, 
            context.Message.GiftcardItem
        });
    }
}