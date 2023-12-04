using System;

namespace NLog.Targets.RedisJson
{
    /// <summary>
    /// Provide interface for redis connection
    /// </summary>
    public interface IRedisConnection : IDisposable
    {
        /// <summary>
        /// Write to redis as json message
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="json">The json</param>
        void WriteJson(string key, string json);

        /// <summary>
        /// Connect to redis
        /// </summary>
        void Connect();

        /// <summary>
        /// Close redis connection
        /// </summary>
        void Close();
    }
}