using MassTransit;

namespace OrderProcessor.State;

public class CardProcessingState : SagaStateMachineInstance, ISagaVersion
{
    public CardProcessingState(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    
}