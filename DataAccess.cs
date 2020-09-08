using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoCrud
{
    public class DataAccess
    {
        public IMongoDatabase db { get; set; }
        private string database;
        public DataAccess(IDatabaseSettings databaseSettings)
        {
            MongoClient client;
            if (String.IsNullOrEmpty(databaseSettings.ConnectionString))
            {
                //empty conntection for localhost 
                client = new MongoClient();
            }
            else
            {
                //live atlas config
                client = new MongoClient(databaseSettings.ConnectionString);
            }

            db = client.GetDatabase(databaseSettings.DatabaseName);
        }



        public async Task CreateAsync<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            await collection.InsertOneAsync(record);
        }


        public void Create<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> Read<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T ReadById<T>(string table, string id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).First();
        }

      


        public void Upsert<T>(string table, string id, T record)
        {
            var collection = db.GetCollection<T>(table);
            ReplaceOptions replaceOptions = new ReplaceOptions { IsUpsert = true };
            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = collection.ReplaceOne(filter,
                         record,
                         replaceOptions);

        }

        //comment 
        public void Delete<T>(string table, string id, T record) {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }

       
    }
}
