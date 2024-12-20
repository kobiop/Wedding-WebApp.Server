using MongoDB.Driver;
using WeddingUI.Server.Models;
using Microsoft.Extensions.Options;
namespace WeddingUI.Server.Services
{
    public class CounterService
    {
        private readonly IMongoCollection<Counter> _counterCollection;
        public CounterService(IOptions<MongoDBSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.connectionString);
            var database = client.GetDatabase(mongoSettings.Value.databaseName);
            _counterCollection = database.GetCollection<Counter>("counter"); 
        }
        public async Task<int> GetNextPhotoId()
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, "photoId");
            var counter = await _counterCollection.Find(filter).FirstOrDefaultAsync();

            if (counter == null)
            {
                counter = new Counter { Id = "photoId", Seq = 1 };
                await _counterCollection.InsertOneAsync(counter);
            }

            // Increment the counter and return the new value
            var update = Builders<Counter>.Update.Inc(c => c.Seq, 1);
            await _counterCollection.UpdateOneAsync(filter, update);
            return counter.Seq;
        }

    }
}
