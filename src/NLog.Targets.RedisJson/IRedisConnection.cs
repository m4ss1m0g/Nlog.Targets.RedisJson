using System;

namespace NLog.Targets.RedisJson
{
    public interface IRedisConnection : IDisposable
    {
        void WriteJson(string key, string json);

        void Connect();

        void Close();
    }
}