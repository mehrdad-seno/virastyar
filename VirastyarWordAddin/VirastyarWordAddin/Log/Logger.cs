using System;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using YAXLib;
using System.Globalization;
using System.Diagnostics;

namespace VirastyarWordAddin.Log
{
    public static class LogHelper
    {
        #region Ctor, Init, ...

        private static readonly Logger s_defaultLogger;

        static LogHelper()
        {
            s_defaultLogger = LogManager.GetLogger("Virastyar");
        }

        public static void InitUserLevelLogger()
        {
            string filePath = SettingsHelper.GetUserDataPath();

            // TODO
            throw new NotImplementedException();
        }

        public static void InitAppLevelLogger()
        {
            string filePath = SettingsHelper.GetCommonDataPath();

            // TODO
            throw new NotImplementedException();
        }
        
        #endregion

        #region Samples for Configuration API

        public static void AddLogger(string className, string methodName, LogLevel minLevel)
        {
            // Step 1. 
            LoggingConfiguration config = LogManager.Configuration;

            // Step 2. Create target and add it to the configuration 
            MethodCallTarget methodCallTarget = new MethodCallTarget();
            config.AddTarget("methodcalltarget", methodCallTarget);

            // Step 3. Set target properties
            methodCallTarget.ClassName = className;
            methodCallTarget.MethodName = methodName;
            methodCallTarget.Parameters.Add(new MethodCallParameter(
                "[${date:format=yyyy\\/MM\\/dd HH\\:mm\\:ss}] [${logger}] ${message}\r\n"));

            config.LoggingRules.Add(new LoggingRule("*", minLevel, methodCallTarget));
            LogManager.ReconfigExistingLoggers();
        }

        public static void AddLogger(Control loggerControl, LogLevel minLevel)
        {
            // Step 1. 
            LoggingConfiguration config = LogManager.Configuration;

            // Step 2. Create target and add it to the configuration 
            FormControlTarget formControlTarget = new FormControlTarget();
            config.AddTarget("formcontrol", formControlTarget);

            // Step 3. Set target properties
            formControlTarget.ControlName = loggerControl.Name;
            formControlTarget.FormName = loggerControl.FindForm().Name;
            formControlTarget.Layout = "[${date:format=yyyy\\/MM\\/dd HH\\:mm\\:ss}] [${logger}] ${message}\r\n";

            config.LoggingRules.Add(new LoggingRule("*", minLevel, formControlTarget));
            LogManager.ReconfigExistingLoggers();
        }

        public static void AddLogger(string logFilePath, LogLevel minLevel)
        {
            // Step 1. 
            LoggingConfiguration config = LogManager.Configuration;

            // Step 2. Create target and add it to the configuration 
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = logFilePath;
            fileTarget.Layout = "[${date:format=yyyy\\/MM\\/dd HH\\:mm\\:ss}] [${logger}] ${message} \r\n ${exception:format=tostring}";

            config.LoggingRules.Add(new LoggingRule("*", minLevel, fileTarget));
            LogManager.ReconfigExistingLoggers();
        }

        #endregion

        #region Logging Helpers

        #region Debug

        public static void Debug(string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, s_defaultLogger.Name, message);

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Debug(string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, s_defaultLogger.Name, message);
            logEventInfo.Keywords = keywords;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Debug(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, message);


            logger.Log(logEventInfo);
        }

        public static void Debug(Logger logger, string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, message);

            logEventInfo.Keywords = keywords;

            logger.Log(logEventInfo);
        }

        public static void Debug(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);


            logger.Log(logEventInfo);
        }

        public static void Debug(Logger logger, string keywords, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);

            logger.Log(logEventInfo);
        }

        public static void DebugException(Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, s_defaultLogger.Name, "");
            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void DebugException(string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, s_defaultLogger.Name, message);
            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void DebugException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;            
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #region Info

        public static void Info(string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, s_defaultLogger.Name, message);


            s_defaultLogger.Log(logEventInfo);
        }

        public static void Info(string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, s_defaultLogger.Name, message);

            logEventInfo.Keywords = keywords;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Info(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, logger.Name, message);


            logger.Log(logEventInfo);
        }

        public static void Info(Logger logger, string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, logger.Name, message);

            logEventInfo.Keywords = keywords;

            logger.Log(logEventInfo);
        }

        public static void Info(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);


            logger.Log(logEventInfo);
        }

        public static void Info(Logger logger, string keywords, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);

            logEventInfo.Keywords = keywords;

            logger.Log(logEventInfo);
        }

        public static void InfoException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        public static void InfoException(Logger logger, string keywords, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Info, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured + "," + keywords;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #region Error

        public static void Error(string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, s_defaultLogger.Name, message);

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Error(string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, s_defaultLogger.Name, message);
            logEventInfo.Keywords = keywords;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Error(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, logger.Name, message);

            logger.Log(logEventInfo);
        }

        public static void Error(Logger logger, string keywords, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, logger.Name, message);
            logEventInfo.Keywords = keywords;

            logger.Log(logEventInfo);
        }

        public static void Error(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);

            logger.Log(logEventInfo);
        }

        public static void ErrorException(Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, s_defaultLogger.Name, "");

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void ErrorException(string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, s_defaultLogger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void ErrorException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #region Fatal

        public static void Fatal(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Fatal, logger.Name, message);


            logger.Log(logEventInfo);
        }

        public static void Fatal(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);


            logger.Log(logEventInfo);
        }
        
        public static void FatalException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Fatal, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #region Warn

        public static void Warn(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Warn, logger.Name, message);


            logger.Log(logEventInfo);
        }

        public static void Warn(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);


            logger.Log(logEventInfo);
        }

        public static void WarnException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Warn, logger.Name, message);

            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #region Trace

        public static void Trace(string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Trace, s_defaultLogger.Name, message);
            ////logEventInfo.SetStackTrace(new StackTrace(true), 1);

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Trace(Logger logger, string message)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Trace, logger.Name, message);

            logger.Log(logEventInfo);
        }

        public static void Trace(string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, s_defaultLogger.Name, CultureInfo.CurrentCulture, message, args);

            s_defaultLogger.Log(logEventInfo);
        }

        public static void Trace(Logger logger, string message, params object[] args)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentCulture, message, args);

            logger.Log(logEventInfo);
        }

        public static void TraceException(Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Trace, s_defaultLogger.Name, "");
            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void TraceException(string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Trace, s_defaultLogger.Name, message);
            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            s_defaultLogger.Log(logEventInfo);
        }

        public static void TraceException(Logger logger, string message, Exception exception)
        {
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Trace, logger.Name, message);
            logEventInfo.Keywords = Constants.LogKeywords.ExceptionOccured;
            logEventInfo.Exception = exception;

            logger.Log(logEventInfo);
        }

        #endregion

        #endregion
    }

    [Layout("XmlLayout")]
    public class XmlLayout : Layout
    {
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            if (logEvent is VirastyarLogEventInfo)
            {
                var serializer = new YAXSerializer(logEvent.GetType());
                try
                {
                    return serializer.Serialize(logEvent);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return "Error in serialization: " + Environment.NewLine + ex;
                }
            }

            return "Unexpected log class";
        }
    }
}
