using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Quantion.MongoDbLogger
{
    /// <summary>
    /// 
    /// </summary>
    [ProviderAlias("MongoDbLogger")]
    public class MongoDbLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, MongoDbLogger> _loggers = new ConcurrentDictionary<string, MongoDbLogger>();
        private readonly MongoDbLoggerConfiguration _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public MongoDbLoggerProvider(MongoDbLoggerConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new MongoDbLogger(name, _config));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
