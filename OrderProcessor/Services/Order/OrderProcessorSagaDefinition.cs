using MassTransit;
using OrderProcessor.State;

namespace OrderProcessor.Services.Order;

public class OrderProcessorSagaDefinition : SagaDefinition<OrderProcessorState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderProcessorState> sagaConfigurator)
    {
        if (endpointConfigurator is IServiceBusEndpointConfigurator sb)
            sb.RequiresSession = true;
    }
}