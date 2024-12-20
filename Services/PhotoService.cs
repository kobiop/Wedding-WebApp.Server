using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WeddingUI.Server.Models;

namespace WeddingUI.Server.Services
{
    public class PhotoService
    {

        private readonly IMongoCollection<Photo> _photoCollection;
        public PhotoService(IOptions<MongoDBSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.connectionString);
            var database = client.GetDatabase(mongoSettings.Value.databaseName);
            _photoCollection = database.GetCollection<Photo>("wedding_photos"); // Collection name
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                // Retrieve a list of database names to verify the connection
                var databases = await _photoCollection.Database.Client.ListDatabaseNamesAsync();
                await databases.MoveNextAsync(); // Check if any databases are returned
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Photo>> GetPhotosAsync()
        {
            var photos = await _photoCollection.Find(_ => true).ToListAsync();
            return photos;
        }

        public async Task InsertPhotoAsync(Photo photo)
        {
            try
            {
                // Insert the photo asynchronously into the collection
                await _photoCollection.InsertOneAsync(photo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting photo into database: {ex.Message}");
            }
        }

        public async Task DeletePhotoAsync(int id)
        {
            var filter = Builders<Photo>.Filter.Eq(p => p.AutoIncrementId, id);
            await _photoCollection.DeleteOneAsync(filter);
        }


    }
}
