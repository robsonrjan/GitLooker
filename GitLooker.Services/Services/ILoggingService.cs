using NLog;

namespace GitLooker.Services.Services
{
    public interface ILoggingService<T>
    {
        LogFactory Factory { get; }
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }

        void Debug(string message);
        void Debug(string message, params object[] args);
        void Error(string message);
        void Error(string message, params object[] args);
        void Exception(string message);
        void Exception(string message, params object[] args);
        void Exception(Exception exception, string message, params object[] args);
        void Fatal(string message);
        void Fatal(string message, params object[] args);
        void Info(string message);
        void Info(string message, params object[] args);
        bool IsEnabled(LogLevel level);
        void Trace(string message);
        void Trace(string message, params object[] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
    }
}