using Common.Commands;
using Common.Events;
using MassTransit;
using Serilog;

namespace OrderProcessor.Consumers.Order;

public class CreateReceiptConsumer : IConsumer<CreateReceipt>
{
    public async Task Consume(ConsumeContext<CreateReceipt> context)
    {
        Log.Information("Consumer: Create order receipt, order id: {OrderId}", context.Message.Order.OrderId);

        await context.Publish<IReceiptGenerated>(new
        {
            context.Message.CorrelationId,
            context.Message.Order
        });
    }
}