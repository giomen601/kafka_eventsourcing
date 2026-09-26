namespace Common.Core.Events;

public class TicketCreatedEvent : BaseEvent
{
    public TicketCreatedEvent() : base(nameof(TicketCreatedEvent))
    {
    }

    public required string UserName { get; set; }
    public int TypeError { get; set; }
    public required string DetailError { get; set; }
}