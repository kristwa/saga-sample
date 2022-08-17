using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers;

public class PaymentVerifierConsumer : IConsumer<VerifyPayment>
{
    public async Task Consume(ConsumeContext<VerifyPayment> context)
    {
        Log.Information("Verify payment for order {Order}", context.Message.Order.OrderId);

        await context.Publish<IPaymentVerified>(new
        {
            CorrelationId = context.Message.CorrelationId,
            Order = context.Message.Order
        });
    }
}