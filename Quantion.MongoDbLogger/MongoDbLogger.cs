using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

namespace Quantion.MongoDbLogger
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoDbLogger : ILogger
    {
        private readonly string _name;
        private readonly MongoDbLoggerConfiguration _config;

        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Log> _logCollection;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="name"></param>
        public MongoDbLogger(string name, MongoDbLoggerConfiguration config)
        {
            MongoClient client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
            _logCollection = _database.GetCollection<Log>(config.CollectionName);
            _config = config;
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Log logger = new Log()
            {
                LogLevel = logLevel.ToString(),
                EventId = eventId.Id,
                Name = _name,
                CreatedOn = System.DateTime.Now,
                Message = formatter(state, exception)
            };

            _logCollection.InsertOne(logger);
        }
    }

    /// <summary>
    /// MongoDb Logger extension methods 
    /// </summary>
    public static class MongoDbLoggerExtensions
    {
        /// <summary>
        /// Extensión Method to add provider
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ILoggerFactory AddMongoDbLogger(this ILoggerFactory loggerFactory, MongoDbLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new MongoDbLoggerProvider(config));
            return loggerFactory;
        }
        /// <summary>
        /// Extension Method to add provider
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static ILoggerFactory AddMongoDbLogger(this ILoggerFactory loggerFactory)
        {
            var config = new MongoDbLoggerConfiguration();
            return loggerFactory.AddMongoDbLogger(config);
        }
        /// <summary>
        /// Extension method to add provider
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILoggerFactory AddMongoDbLogger(this ILoggerFactory loggerFactory, Action<MongoDbLoggerConfiguration> configure)
        {
            var config = new MongoDbLoggerConfiguration();
            configure(config);
            return loggerFactory.AddMongoDbLogger(config);
        }

    }
}
