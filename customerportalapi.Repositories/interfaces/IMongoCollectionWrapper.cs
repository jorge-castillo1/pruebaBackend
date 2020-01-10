using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IMongoCollectionWrapper<T>
    {
        List<T> FindOne(Expression<Func<T, bool>> filter, FindOptions options = null);
        ReplaceOneResult ReplaceOne(FilterDefinition<T> filter, T replacement);
        void InsertOne(T data);
        Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter);
    }
}
