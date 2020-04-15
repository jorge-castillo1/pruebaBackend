using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace customerportalapi.Services
{
    public class DistributedMongoDbCache<TItem>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;

        public DistributedMongoDbCache(IDistributedCache distributedCache, DistributedCacheEntryOptions cacheEntryOptions)
        {
            _distributedCache = distributedCache;
            _cacheEntryOptions = cacheEntryOptions;
        }

        public async Task<TItem> GetOrCreateCache(string key, Func<Task<TItem>> createItem)
        {
            TItem cacheEntry;
            string cacheKey = key + CacheKeys.Entry;
            byte[] result = _distributedCache.Get(cacheKey);
            if (result == null)
            {
                cacheEntry = await createItem();
                string json = JsonConvert.SerializeObject(cacheEntry);
                byte[] value = Encoding.UTF8.GetBytes(json);

                _distributedCache.Set(cacheKey, value, _cacheEntryOptions);
            }
            else
            {
                string content = Encoding.UTF8.GetString(result);
                cacheEntry = JsonConvert.DeserializeObject<TItem>(content);
            }

            return cacheEntry;
        }
    }

    public class CacheKeys
    {
        public static string Entry => "_Entry";
        public static string CallbackEntry => "_Callback";
        public static string CallbackMessage => "_CallbackMessage";
        public static string Parent => "_Parent";
        public static string Child => "_Child";
        public static string DependentMessage => "_DependentMessage";
        public static string DependentCts => "_DependentCTS";
        public static string Ticks => "_Ticks";
        public static string CancelMsg => "_CancelMsg";
        public static string CancelTokenSource => "_CancelTokenSource";
    }
}
