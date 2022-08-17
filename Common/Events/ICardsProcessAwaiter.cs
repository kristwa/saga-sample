namespace Common.Events;

public interface ICardsProcessAwaiter
{
    public Guid OrderId { get; set; }
}