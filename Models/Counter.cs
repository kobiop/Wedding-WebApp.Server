using MongoDB.Bson.Serialization.Attributes;

namespace WeddingUI.Server.Models
{
    public class Counter
    {
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("seq")]
        public int Seq { get; set; }
    }
}
