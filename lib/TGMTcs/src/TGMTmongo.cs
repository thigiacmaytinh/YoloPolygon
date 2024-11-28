using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGMTcs
{
    class TGMTmongo
    {
        MongoClient m_client;
        public IMongoDatabase m_database;
        static TGMTmongo m_instance;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static TGMTmongo GetInstance()
        {
            if (m_instance == null)
                m_instance = new TGMTmongo();
            return m_instance;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public bool Init(string server, string dbName, string port="27017")
        {
            string connectionString = "mongodb://" + server + ":" + port;
            m_client = new MongoClient(connectionString);

            m_database = m_client.GetDatabase(dbName);
            return m_database != null;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PrintDBlist()
        {
            var dbList = m_client.ListDatabases().ToList();
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Insert()
        {
            var document = new BsonDocument
            {
                { "farmID", 10000 },
                { "address", 10000 },
                { "ownerName", 10000 },
            };

            var collection = m_database.GetCollection<BsonDocument>("farm");
            collection.InsertOne(document);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public BsonDocument Insert(string collectionName, BsonDocument document)
        {
            var collection = m_database.GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(document);
            return document;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InsertAsync(string collectionName, BsonDocument document)
        {
            var collection = m_database.GetCollection<BsonDocument>(collectionName);
            collection.InsertOneAsync(document);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Read()
        {
            var collection = m_database.GetCollection<BsonDocument>("farm");
            var filter = Builders<BsonDocument>.Filter.Eq("farmID", "CM01");
            var studentDocument = collection.Find(filter).FirstOrDefault();
            Console.WriteLine(studentDocument.ToString());
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public BsonDocument Query(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = m_database.GetCollection<BsonDocument>(collectionName);
            var document = collection.Find(filter).FirstOrDefault();
            return document;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IMongoCollection< BsonDocument> GetCollection(string collectionName)
        {
            var collection = m_database.GetCollection<BsonDocument>(collectionName);
            return collection;
        }
    }
}
