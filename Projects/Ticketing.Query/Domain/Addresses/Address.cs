using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketing.Query.Domain.Addresses;

[ComplexType] //se agrega esto para asignarlo como un value object
public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}