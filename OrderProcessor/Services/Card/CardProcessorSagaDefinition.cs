using MassTransit;
using OrderProcessor.State;

namespace OrderProcessor.Services.Card;

public class CardProcessorSagaDefinition : SagaDefinition<CardProcessingState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CardProcessingState> sagaConfigurator)
    {
        if (endpointConfigurator is IServiceBusEndpointConfigurator sb)
            sb.RequiresSession = true;
    }
}