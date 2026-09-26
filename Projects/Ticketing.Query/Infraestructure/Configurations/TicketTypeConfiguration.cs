using Common.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infraestructure.Configurations;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_type");
        builder.HasKey(x => x.Id);

        IEnumerable<TicketType> ticketTypes = Enum.GetValues<TicketTypeEnum>().Select(p => TicketType.Create((int)p));

        builder.HasData(ticketTypes);
    }
}