using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Photo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // This allows MongoDB to handle ObjectId conversion.
    public ObjectId Id { get; set; }

    [BsonElement("_Id")] // Auto-incremented ID as a separate field
    public int AutoIncrementId { get; set; }
    [BsonElement("filename")]
    public string Filename { get; set; }
    [BsonElement("userId")]
    public string UserId { get; set; } // UserId to associate with the photo
    [BsonElement("photo")]
    public byte[] PhotoData { get; set; }
}
