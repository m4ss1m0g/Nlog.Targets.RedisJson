using System;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets.RedisJson
{
    [Target("RedisJson")]
    public class RedisJsonTarget : TargetWithLayout
    {
        /// <summary>
        /// The time to live of the item
        /// </summary>
        private TimeSpan? _ttl;

        /// <summary>
        /// The managed redis connection
        /// </summary>
        private IRedisConnection _redisConnection;

        /// <summary>
        /// Sets the host name or IP Address of the redis server
        /// </summary>
        [RequiredParameter]
        public string Host { get; set; }

        /// <summary>
        /// Sets the port number redis is running on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Sets the database id to be used in redis
        /// </summary>
        public int Db { get; set; }

        /// <summary>
        /// Set the password to be used in redis
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The key prefix for json entries
        /// </summary>
        public Layout ItemKey { get; set; }
        /// </summary>

        /// <summary>
        /// dd,hh,mm,ss,ms
        /// https://learn.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=net-8.0
        /// </summary>
        public string TTL { get; set; }

        /// <summary>
        /// The redis configuration options see https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        /// </summary>
        public string ConfigurationOptions { get; set; }

        /// <summary>
        /// Creates a new instance of the RedisJsonTarget
        /// </summary>
        public RedisJsonTarget()
        {
            Host = string.Empty;
            Port = 6379;
            Db = 0;
        }

        /// <summary>
        /// Initializes the target, read the host, password and port
        /// </summary>
        /// <exception cref="NLogConfigurationException"></exception>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(Host))
                throw new NLogConfigurationException("Host cannot be null");

            if (TTL != null)
            {
                if (TimeSpan.TryParse(TTL, out var ttl))
                {
                    _ttl = ttl;
                }
                else
                {
                    throw new NLogConfigurationException($"Unable to parse TTL to TimeSpan (see: https://learn.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=net-8.0): {TTL}");
                }
            }

            InternalLogger.Info($"Host: {Host}, Port: {Port}, Db: {Db}, HasPassword: {Password != null}, TTL: {_ttl}, ConfigurationOptions: {ConfigurationOptions}");

            _redisConnection = new RedisConnection(Host, Port, Db, Password, _ttl, ConfigurationOptions);
            _redisConnection.Connect();
        }

        /// <summary>
        /// Closes the target
        /// </summary>
        protected override void CloseTarget()
        {
            base.CloseTarget();
            _redisConnection?.Close();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _redisConnection?.Dispose();
        }

        /// <summary>
        /// Writes the logging event to redis
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (Layout.GetType() == typeof(JsonLayout))
            {
                WriteJsonLayout(logEvent);
            }
            else
            {
                WriteMessageLayout(logEvent);
            }
        }

        /// <summary>
        /// Writes the message whitout attributes
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        private void WriteMessageLayout(LogEventInfo logEvent)
        {
            var msg = new
            {
                Level = logEvent.Level.Name.ToUpper(),
                logEvent.Exception,
                logEvent.SequenceID,
                logEvent.TimeStamp,
                logEvent.StackTrace,
                Message = RenderLogEvent(Layout, logEvent),
            };

            string key = ItemKey?.Render(logEvent) ?? $"log_{DateTime.Now.Ticks}";
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);

            _redisConnection?.WriteJson(key, json);
        }

        /// <summary>
        /// Write the log event as the JSON from attributes
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        private void WriteJsonLayout(LogEventInfo logEvent)
        {
            string key = ItemKey?.Render(logEvent) ?? $"log_{DateTime.Now.Ticks}";
            string logMessage = RenderLogEvent(Layout, logEvent);

            _redisConnection?.WriteJson(key, logMessage);
        }

    }
}