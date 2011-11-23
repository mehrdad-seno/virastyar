using System;
using System.Net;
using System.Threading;
using NLog.Targets;
using VirastyarWordAddin.Properties;
using NLog;
using YAXLib;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace VirastyarWordAddin.Log
{
    public class LogInfos
    {
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "VirastyarLogEventInfo")]
        public VirastyarLogEventInfo[] LogEntries { get; set; }
    }

    public static class LogReporter
    {
        private static readonly string Submitted = "SUBMITTED";
        private static readonly string NotSubmitted = "NOTSUBMITTED";

        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void AutomaticReport()
        {
            if (Settings.Default.LogReport_AutomaticReport && Settings.Default.LogReport_ConfirmationDone)
                AutomaticReport(false, 15000);
        }

        public static void AutomaticReport(bool force)
        {
            AutomaticReport(force, 0);
        }

        public static void AutomaticReport(bool force, int delay)
        {
            int reportInterval = Settings.Default.LogReport_AutomaticReportInterval;
            DateTime lastReport = Settings.Default.LogReport_LastSentLog;

            if (force || lastReport.AddDays(reportInterval).Date <= DateTime.Now.Date)
            {
                try
                {
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            Thread.Sleep(delay);
                            if (SendReport())
                            {
                                Settings.Default.LogReport_LastSentLog = DateTime.Now;
                                Settings.Default.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.DebugException(s_logger, "Error in sending log report", ex);
                        }
                    });
                    thread.Start();
                }
                catch(Exception ex)
                {
                    LogHelper.DebugException(s_logger, "Error in sending log report", ex);
                }
            }
        }

        private static bool SendReport()
        {
            var target = LogManager.Configuration.FindTargetByName("VirastyarLogFile");

            if (target is FileTarget)
            {
                try
                {
                    var fileTarget = (FileTarget) target;

                    string logFilePath = fileTarget.FileName.Render(null);
                    if (File.Exists(logFilePath))
                    {
                        if (Send(logFilePath))
                        {
                            File.Delete(logFilePath);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException(s_logger, "Exception while trying to send the log report", ex);
                }
            }

            return false;
        }

        public static string GetLogPath()
        {
            var target = LogManager.Configuration.FindTargetByName("VirastyarLogFile");

            if (target is FileTarget)
            {
                try
                {
                    var fileTarget = (FileTarget)target;

                    string logFilePath = fileTarget.FileName.Render(null);
                    return Path.GetDirectoryName(logFilePath);
                }
                catch (Exception)
                {
                }
            }

            return "";
        }

        public static LogEventInfo[] LoadLog(string logFilePath)
        {
            string content = File.ReadAllText(logFilePath);
            content = String.Format("<LogInfos>\r\n{0}\r\n</LogInfos>", content);
            
            var serializer = new YAXSerializer(typeof(LogInfos), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                                                  YAXExceptionTypes.Error);

            var logInfos = (LogInfos)serializer.Deserialize(content);
            return logInfos.LogEntries;
        }

        public static bool Send(VirastyarLogEventInfo[] logEventInfo)
        {
            bool result = false;
            var serializer = new YAXSerializer(typeof(VirastyarLogEventInfo), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error);
            // Creat a temp file
            string tempFilePath = Path.GetTempFileName();
            try
            {
                using(var writer = new StreamWriter(tempFilePath))
                {
                    foreach (var eventInfo in logEventInfo)
                    {
                        string serialized = serializer.Serialize(eventInfo);
                        writer.WriteLine(serialized);
                    }
                }

                result = Send(tempFilePath);
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            try
            {
                File.Delete(tempFilePath);
            }
            catch
            {
                // Ignore
            }
            return result;
        }

        public static bool Send(string logFilePath)
        {
            WebRequest request = WebRequest.Create(Settings.Default.LogReport_Url);
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Method = "POST";
            request.ContentType = "application/x-gzip";

            using (var fstream = File.OpenRead(logFilePath))
            {
                using (var mstream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(mstream, CompressionMode.Compress))
                    {

                        var buffer = new byte[1024];
                        while (true)
                        {
                            int len = fstream.Read(buffer, 0, buffer.Length);
                            if (len == 0) break;
                            gZipStream.Write(buffer, 0, len);
                        }

                        gZipStream.Close();
                        mstream.Close();

                        var buf = mstream.ToArray();

                        request.ContentLength = buf.Length;
                        request.GetRequestStream().Write(buf, 0, (int) request.ContentLength);
                    }
                }

                WebResponse response = request.GetResponse();
                Debug.Assert(response != null);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        try
                        {
                            var result = responseReader.ReadToEnd();
                            LogHelper.Trace(result);
                            if (result.ToUpper().Contains(NotSubmitted.ToUpper()))
                                return false;

                            return true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.DebugException("", ex);
                            return false;
                        }
                    }
                }
            }
        }

        private static long CopyTo(Stream source, Stream destination, int bufferSize)
        {
            var gZipStream = new GZipStream(destination, CompressionMode.Compress);
            int num;
            var buffer = new byte[bufferSize];
            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                gZipStream.Write(buffer, 0, num);
            }

            return 0;
        }
    }
}
