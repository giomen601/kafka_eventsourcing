using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Core.Events;

namespace Ticketing.Query.Infraestructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type type)
    {
        return type.IsAssignableFrom(typeof(BaseEvent));
    }

    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
            throw new JsonException($"Fallo el parseo json de {nameof(JsonDocument)}");

        if (!doc.RootElement.TryGetProperty("Type", out var type))
            throw new JsonException($"No se puede parsear la propiedad {doc.GetType()}");

        var typeDiscriminator = type.GetString();
        string json = doc.RootElement.GetRawText();

        return typeDiscriminator switch
        {
            nameof(TicketCreatedEvent) => JsonSerializer.Deserialize<TicketCreatedEvent>(json, options),
            nameof(TicketUpdatedEvent) => JsonSerializer.Deserialize<TicketUpdatedEvent>(json, options),
            _ => throw new JsonException($"{typeDiscriminator} no soportado")
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}