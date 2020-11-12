namespace Quantion.MongoDbLogger
{
    /// <summary>
    /// Logger Configuration Class
    /// </summary>
    public class MongoDbLoggerConfiguration
    {
        /// <summary>
        /// ConnectionString Member
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Database name member
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// Collection name
        /// </summary>
        public string CollectionName { get; set; } = "logs";
    }
}
