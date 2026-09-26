using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infraestructure.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TicketType)
        .HasConversion(tp => tp!.Id, value => TicketType.Create(value));

        builder.HasMany(x => x.Employees)
        .WithMany(x => x.Tickets)
        .UsingEntity<TicketEmployee>
        (
            j => j
                .HasOne(p => p.Employee)
                .WithMany(p => p.TicketEmployees)
                .HasForeignKey(p => p.EmployeeId),
            j => j
                .HasOne(p => p.Ticket)
                .WithMany(p => p.TicketEmployees)
                   .HasForeignKey(p => p.TicketId),
            j =>
                {
                    j.HasKey(t => new { t.TicketId, t.EmployeeId });
                }
        );
    }
}