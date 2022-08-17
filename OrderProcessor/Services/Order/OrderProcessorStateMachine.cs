using Common.Commands;
using Common.Events;
using MassTransit;
using OrderProcessor.State;
using Serilog;

namespace OrderProcessor.Services.Order;

public class OrderProcessorStateMachine : MassTransitStateMachine<OrderProcessorState>
{
    public OrderProcessorStateMachine()
    {
        InstanceState(x => x.CurrentState);
        this.ConfigureCorrelationIds();

        Initially(
            When(CardsProcessed)
                .TransitionTo(Processing)
                .Then(ctx => Log.Information("Order Saga - Step 1: Saga Order Fullfilment Process started for order: {Order}", ctx.Message.Order.OrderId))
                .PublishAsync(ctx => ctx.Init<CreateReceipt>(new CreateReceipt()
                    { CorrelationId = ctx.Message.CorrelationId, Order = ctx.Message.Order }))
        );
        
        During(Processing,
            When(ReceiptGenerated)
                .Then(ctx => Log.Information("Order Saga - Step 2: Receipt generated, send it to recipient"))
                .PublishAsync(ctx => ctx.Init<SendReceipt>(new SendReceipt() { CorrelationId = ctx.Message.CorrelationId, Order = ctx.Message.Order})));
        
        During(Processing,
            When(ReceiptSent)
                .Then(ctx => Log.Information("Order Saga - Step 3: Receipt sent, finalize order"))
                .Finalize());
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => CardsProcessed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => ReceiptGenerated, x => x.CorrelateById((c => c.Message.CorrelationId)));
        Event(() => ReceiptSent, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    public MassTransit.State Processing { get; set; }

    public Event<CardsProcessed> CardsProcessed { get; private set; }
    public Event<IReceiptGenerated> ReceiptGenerated { get; private set; }
    public Event<IReceiptEmailSent> ReceiptSent { get; private set; }
    


    // Entry: All cards have been filled and processed 
    // 1. Create receipt
    // 2. Email receipt
    // 3. Finish Order
}