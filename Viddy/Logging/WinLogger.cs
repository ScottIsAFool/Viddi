// Usage: 
// private static readonly ILog Logger = new Logger(typeof(YOUR_CLASS_NAME));
// 
// In your method:
// Logger.Log("Application_Launching");

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;

namespace ScottIsAFool.WindowsPhone.Logging
{
    public interface ILog
    {
        void Info(string message, params object[] parameters);
        void InfoException(string message, Exception exception);
        void Warning(string message, params object[] parameters);
        void WarningException(string message, Exception exception);
        void Error(string message, params object[] parameters);
        void ErrorException(string message, Exception exception);
        void Fatal(string message, params object[] parameters);
        void FatalException(string message, Exception exception);
        void Debug(string message, params object[] parameters);
        void DebugException(string message, Exception exception);
        void Trace(string message, params object[] parameters);
        void TraceException(string message, Exception exception);
    }

    public class NullLogger : ILog
    {
        public void Info(string message, params object[] parameters)
        {
        }

        public void InfoException(string message, Exception exception)
        {
        }

        public void Warning(string message, params object[] parameters)
        {
        }

        public void WarningException(string message, Exception exception)
        {
        }

        public void Error(string message, params object[] parameters)
        {
        }

        public void ErrorException(string message, Exception exception)
        {
        }

        public void Fatal(string message, params object[] parameters)
        {
        }

        public void FatalException(string message, Exception exception)
        {
        }

        public void Debug(string message, params object[] parameters)
        {
        }

        public void DebugException(string message, Exception exception)
        {
        }

        public void Trace(string message, params object[] parameters)
        {
        }

        public void TraceException(string message, Exception exception)
        {
        }
    }

    public enum LogType
    {
        WriteToFile,
        InMemory
    }

    public interface ILogConfiguration
    {
        bool LoggingIsEnabled { get; set; }
        int MaxNumberOfDays { get; set; }
        int NumberOfRecords { get; set; }
        LogType LogType { get; set; }
    }

    public class LogConfiguration : ILogConfiguration
    {
        public LogConfiguration()
        {
            MaxNumberOfDays = 5;
            LogType = LogType.WriteToFile;
            NumberOfRecords = 100;
        }

        public bool LoggingIsEnabled { get; set; }
        public int MaxNumberOfDays { get; set; }
        public int NumberOfRecords { get; set; }
        public LogType LogType { get; set; }
    }

    public class WinLogger : ILog
    {
        private static readonly IStorageServiceHandler CacheStorage;
        private readonly string _typeName;

        #region Public static properties
        public static string LogFileName { get { return "TraceLog.txt"; } }
        public static LogConfiguration LogConfiguration { get; set; }
        public static string AppVersion { get; set; }
        #endregion

        #region Private static properties
        private static readonly List<string> LogList = new List<string>();
        #endregion

        #region Constructors
        public WinLogger(Type type)
            : this(type.FullName)
        {
        }

        static WinLogger()
        {
            if (LogConfiguration == null)
            {
                LogConfiguration = new LogConfiguration();
            }

            if (CacheStorage == null)
            {
                CacheStorage = new StorageService().LocalCache;
            }
        }

        public WinLogger(string typeName)
        {
            _typeName = typeName;
            if (LogConfiguration == null)
            {
                LogConfiguration = new LogConfiguration();
            }
        }
        #endregion

        private void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            var messageLog = new StringBuilder();

            messageLog.Append("[ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ]");
            messageLog.Append("[" + logLevel.ToString().ToUpper() + "]");
            messageLog.Append("[ " + _typeName + " ]");
            messageLog.AppendFormat("[ Message: {0} ]", message);

            System.Diagnostics.Debug.WriteLine(messageLog);

            if (!LogConfiguration.LoggingIsEnabled) return;

            if (LogConfiguration.LogType == LogType.InMemory)
            {
                if (LogList.Count == LogConfiguration.NumberOfRecords)
                {
                    LogList.RemoveAt(0);
                }

                LogList.Add(messageLog.ToString());
            }
            else
            {
                WriteLogToFile(messageLog);
            }
        }

        private static async Task WriteLogToFile(StringBuilder messageLog)
        {
            try
            {

                //if (await CacheStorage.FileExistsAsync(LogFileName))
                //{
                //    // Delete log file if older than 5 days or told to delete it (ie, dump)
                //    if ((DateTime.Now.Subtract(store.GetCreationTime(LogFileName).DateTime).TotalDays > LogConfiguration.MaxNumberOfDays
                //         || deleteFileFirst))
                //    {
                //        store.DeleteFile(LogFileName);
                //        fileStream = store.CreateFile(LogFileName);
                //    }
                //}
                //else
                //{
                //    fileStream = store.CreateFile(LogFileName);
                //}

                //if (fileStream == null)
                //{
                //    fileStream = store.OpenFile(LogFileName, FileMode.Append, FileAccess.Write, FileShare.None);
                //}

                //using (TextWriter output = new StreamWriter(fileStream))
                //{
                //    output.WriteLine(messageLog);
                //}

                await CacheStorage.WriteAllTextAsync(LogFileName, messageLog.ToString());
            }
            catch (Exception)
            {
            }
        }

        private void LogFormat(string format, LogLevel logLevel, params object[] parameters)
        {
            try
            {
                Log(string.Format(format, parameters), logLevel);
            }
            catch
            {
                Log("Error writing to log file.");
                Log("Original message: " + format);
            }
        }

        #region Static Methods
        public static async Task<string> GetLogs()
        {
            if (LogConfiguration.LogType == LogType.InMemory)
            {
                var sb = new StringBuilder();

                var versionString = string.Format("App version: {0}" + Environment.NewLine, string.IsNullOrEmpty(AppVersion) ? "Unknown" : AppVersion);
                sb.AppendLine(versionString);

                foreach (var log in LogList)
                {
                    sb.Insert(versionString.Length, log + Environment.NewLine);
                }

                // Dump the file to IsoStorage anyway
                await WriteLogToFile(sb);

                return sb.ToString();
            }

            return await GetLogFileContent();
        }

        public static async Task DumpLogsToFile()
        {
            GetLogs();
        }

        public static Task<string> ReadLogFile()
        {
            return GetLogFileContent();
        }

        private static async Task<string> GetLogFileContent()
        {
            try
            {
                if (await CacheStorage.FileExistsAsync(LogFileName))
                {
                    return await CacheStorage.ReadAllTextAsync(LogFileName);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static async Task ClearLogs()
        {
            try
            {
                if (await CacheStorage.FileExistsAsync(LogFileName))
                {
                    await CacheStorage.DeleteFileAsync(LogFileName);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region LogType Messages
        public void Info(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Info, parameters);
        }

        private void GenerateExceptionMessage(string message, Exception ex, LogLevel logLevel)
        {
            var exceptionMessage = string.Format("App message: {0}\nException: {1}\nException hashcode: {2}\n{3}", message, ex.Message, ex.GetHashCode(), ex.StackTrace);
            Log(exceptionMessage, logLevel);
        }

        public void InfoException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Info);
        }

        public void Warning(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Warning, parameters);
        }

        public void WarningException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Warning);
        }

        public void Error(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Error, parameters);
        }

        public void ErrorException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Error);
        }

        public void Fatal(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Fatal, parameters);
        }

        public void FatalException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Fatal);
        }

        public void Debug(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Debug, parameters);
        }

        public void DebugException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Debug);
        }

        public void Trace(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Trace, parameters);
        }

        public void TraceException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Trace);
        }
        #endregion
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Fatal,
        Debug,
        Trace
    }
}