namespace Common.Core.Events;

public interface IEventProducer
{
    Task ProduceAsync(string topic, BaseEvent @event);
}