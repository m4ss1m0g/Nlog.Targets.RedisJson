using System;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace NLog.Targets.RedisJson
{
    /// <summary>
    /// Provide connection to Redis
    /// </summary>
    internal class RedisConnection : IDisposable, IRedisConnection
    {
        /// <summary>
        /// Redis configuration
        /// </summary>
        private readonly ConfigurationOptions _config;

        /// <summary>
        /// Redis connection
        /// </summary>
        private IConnectionMultiplexer _redis;

        /// <summary>
        /// Redis database
        /// </summary>
        private IDatabase _database;

        /// <summary>
        /// Redis database id
        /// </summary>
        private readonly int _db;

        /// <summary>
        /// The redis key Time to live
        /// </summary>
        private readonly TimeSpan? _ttl;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool _disposedValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConnection"/> class
        /// </summary>
        /// <param name="host">The host</param>
        /// <param name="port">The port</param>
        /// <param name="db">The db id</param>
        /// <param name="password">The optional password</param>
        /// <param name="ttl">The option TTL</param>
        /// <param name="configurationOptions">The optional configuration</param>
        public RedisConnection(string host, int port, int db, string password = null, TimeSpan? ttl = null, string configurationOptions = null)
        {
            _db = db;
            _ttl = ttl;

            _config = string.IsNullOrEmpty(configurationOptions) ? new ConfigurationOptions() : ConfigurationOptions.Parse(configurationOptions);

            _config.EndPoints.Add(host, port);

            if (!string.IsNullOrEmpty(password))
                _config.Password = password;
        }

        /// <summary>
        /// Connect to Redis
        /// </summary>
        public void Connect()
        {
            _redis = ConnectionMultiplexer.Connect(_config);
            _database = _redis.GetDatabase(_db);
        }

        /// <summary>
        /// Close Redis connection
        /// </summary>
        public void Close()
        {
            _redis?.Close();
        }

        /// <summary>
        /// Write the json to Redis
        /// </summary>
        /// <param name="key">The item key</param>
        /// <param name="json">The json payload</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteJson(string key, string json)
        {
            if (_database == null)
                throw new InvalidOperationException("Redis connection not initialized");

            JsonCommands jsonCommand = _database.JSON();
            jsonCommand.Set(key, "$", json);

            if (_ttl != null)
                _database.KeyExpire(key, _ttl);

        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _redis?.Dispose();
                }

                _disposedValue = true;
            }
        }

        // ~RedisConnection()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}