using MongoDB.Bson.Serialization.Attributes;

namespace Common.Core.Messages;

public abstract class Message
{
    public Message()
    {
        
    }
    
    [BsonId]
    public string Id { get; set; } = string.Empty;
}