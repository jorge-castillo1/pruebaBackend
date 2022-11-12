using customerportalapi.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Utils
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

        public List<T> Find(FilterDefinition<T> filter, int pagenum, int pagesize, FindOptions options = null)
        {
            var skip = pagenum == 1 ? 0 : (pagenum - 1) * pagesize;

            return _mongoCollection.Find(filter, options).Skip(skip).Limit(pagesize).ToList();
        }
    }
}
