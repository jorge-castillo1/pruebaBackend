using customerportalapi.Repositories.interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.utils
{
    public class MongoCollectionWrapper<T> : IMongoCollectionWrapper<T>
    {
        readonly IMongoCollection<T> _mongoCollection;

        public MongoCollectionWrapper(IMongoDatabase database, string collectionName)
        {
            _mongoCollection = database.GetCollection<T>(collectionName);
        }

        public List<T> FindOne(Expression<Func<T, bool>> filter, FindOptions options = null)
        {
            return _mongoCollection.Find(filter).Limit(1).ToList();
        }

        public ReplaceOneResult ReplaceOne(FilterDefinition<T> filter, T replacement)
        {
            return _mongoCollection.ReplaceOne(filter, replacement);
        }

        public void InsertOne(T data)
        {
            _mongoCollection.InsertOne(data);
        }

        public async Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter)
        {
            return await _mongoCollection.DeleteOneAsync(filter);
        }

        public List<T> FindAll(Expression<Func<T, bool>> filter)
        {
            return _mongoCollection.Find(filter).ToList();
        }
    }
}
