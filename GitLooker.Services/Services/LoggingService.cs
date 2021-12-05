using NLog;

namespace GitLooker.Services.Services
{
    public class LoggingService<T> : ILoggingService<T>
    {
        private readonly ILogger logger;

        public LoggingService()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public bool IsTraceEnabled => logger.IsTraceEnabled;

        public bool IsDebugEnabled => logger.IsDebugEnabled;

        public bool IsInfoEnabled => logger.IsInfoEnabled;

        public bool IsWarnEnabled => logger.IsWarnEnabled;

        public bool IsErrorEnabled => logger.IsErrorEnabled;

        public bool IsFatalEnabled => logger.IsFatalEnabled;

        public LogFactory Factory => logger.Factory;


        public void Debug(string message, params object[] args) => logger.Debug($"[{typeof(T).Name}] {message}", args);

        public void Debug(string message) => logger.Debug($"[{typeof(T).Name}] {message}");

        public void Error(string message, params object[] args) => logger.Error($"[{typeof(T).Name}] {message}", args);

        public void Error(string message) => logger.Error($"[{typeof(T).Name}] {message}");

        public void Fatal(string message, params object[] args) => logger.Fatal($"[{typeof(T).Name}] {message}", args);

        public void Fatal(string message) => logger.Fatal($"[{typeof(T).Name}] {message}");

        public void Info(string message, params object[] args) => logger.Info($"[{typeof(T).Name}] {message}", args);

        public void Info(string message) => logger.Info($"[{typeof(T).Name}] {message}");

        public bool IsEnabled(LogLevel level) => logger.IsEnabled(level);

        public void Trace(string message, params object[] args) => logger.Trace($"[{typeof(T).Name}] {message}", args);

        public void Trace(string message) => logger.Trace($"[{typeof(T).Name}] {message}");

        public void Warn(string message, params object[] args) => logger.Warn($"[{typeof(T).Name}] {message}", args);

        public void Warn(string message) => logger.Warn($"[{typeof(T).Name}] {message}");

        public void Exception(Exception exception, string message, params object[] args) => logger.Error(exception, $"[{typeof(T).Name}] {message}", args);

        public void Exception(string message, params object[] args) => logger.Error($"[{typeof(T).Name}] {message}", args);

        public void Exception(string message) => logger.Error($"[{typeof(T).Name}] {message}");
    }
}
