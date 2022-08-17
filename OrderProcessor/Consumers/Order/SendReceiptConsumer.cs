using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Order;

public class SendReceiptConsumer : IConsumer<SendReceipt>
{
    public async Task Consume(ConsumeContext<SendReceipt> context)
    {
        Log.Information("Consumer: Send order receipt by email, order: {OrderNo}", context.Message.Order.OrderId);

        await context.Publish<IReceiptEmailSent>(new
        {
            context.Message.CorrelationId,
            context.Message.Order
        });
    }
}