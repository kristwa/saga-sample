using MassTransit;

namespace OrderProcessor.State;

public class OrderProcessorState : SagaStateMachineInstance, ISagaVersion
{
    public OrderProcessorState(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    public string CurrentState { get; set; }
}