using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using NLog;
using YAXLib;
using NLog.LayoutRenderers;
using System.Text;

namespace VirastyarWordAddin.Log
{
    [YAXSerializableType(FieldsToSerialize=YAXSerializationFields.AttributedFieldsOnly)]
    public class VirastyarLogEventInfo : LogEventInfo
    {
        #region ctors

        public VirastyarLogEventInfo()
        {
            Properties = new Dictionary<object, object>();
        }

        public VirastyarLogEventInfo(LogLevel level, string loggerName, [Localizable(false)] string message)
            : base(level, loggerName, null, message, null, null)
        {
            Properties = new Dictionary<object, object>();
        }

        public VirastyarLogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
            : base(level, loggerName, formatProvider, message, parameters, null)
        {
            Properties = new Dictionary<object, object>();
        }

        public VirastyarLogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters, Exception exception)
            : base(level, loggerName, formatProvider, message, parameters)
        {
            Properties = new Dictionary<object, object>();
            Exception = exception;
        }

        #endregion

        #region Fields
        
        [YAXSerializableField]
        public new LogLevel Level
        {
            get { return base.Level; }
            set { base.Level = value; }
        }

        [YAXSerializableField]
        public string Keywords
        { get; set; }

        [YAXSerializableField]
        new public string LoggerName
        {
            get { return base.LoggerName; }
            set { base.LoggerName = value; }
        }

        [YAXSerializableField]
        public string OperatingSystem
        {
            get { return Environment.OSVersion.VersionString; }
        }

        [YAXSerializableField]
        public string VirastyarVersion
        {
            get { return ThisAddIn.InstalledVersion.ToString(); }
        }

        [YAXSerializableField]
        public string OfficeVersion
        {
            get { return ThisAddIn.OfficeVersion; }
        }

        [YAXSerializableField]
        public string Guid
        {
            get { return ThisAddIn.InstallationGuid; }
        }

        public new Exception Exception
        {
            get { return base.Exception; }
            set
            {
                base.Exception = value;
                ExceptionInfo = base.Exception.ToString();
            }
        }

        [YAXSerializableField]
        [YAXCustomSerializer(typeof(CDataCustomSerializer))]
        [YAXCustomDeserializer(typeof(CDataCustomSerializer))]
        public string ExceptionInfo
        {
            get;
            set;
        }

        [YAXSerializableField]
        new public string Message
        {
            get { return base.FormattedMessage; }
            set { base.Message = value; }
        }

        [YAXSerializableField]
        new public IDictionary<object, object> Properties
        {
            get; set;
        }

        [YAXSerializableField]
        new public int SequenceID
        {
            get { return base.SequenceID; }
        }

        [YAXSerializableField]
        [YAXCustomSerializer(typeof(CDataCustomSerializer))]
        [YAXCustomDeserializer(typeof(CDataCustomSerializer))]
        public string StackTraceInfo
        {
            get
            {
                var renderer = new StackTraceLayoutRenderer()
                                   {
                                       Format = StackTraceFormat.DetailedFlat,
                                       TopFrames = 3
                                   };
                try
                {
                    return renderer.Render(this);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        [YAXSerializableField]
        public new DateTime TimeStamp
        {
            get { return base.TimeStamp; }
            set { base.TimeStamp = value; }
        }

        #endregion

        #region Methods

        public void AttachFile(string name, string filePath)
        {
            if (Properties.ContainsKey(name))
                throw new ArgumentException("Another file is already attached with the given name:" + name);

            const int blockSize = 3*1024;
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File not found:" + filePath);
            }
            try
            {
                var encoded = new StringBuilder();
                using (var reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    int read;
                    var buffer = new byte[blockSize];
                    do
                    {
                        read = reader.Read(buffer, 0, blockSize);
                        encoded.Append(Convert.ToBase64String(buffer, 0, read, Base64FormattingOptions.InsertLineBreaks));
                    } while (read == blockSize);
                }
                Properties.Add(name, encoded.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Error in attaching file", ex);
                throw;
            }
        }

        public void SaveAttachedFile(string name, string targetFilePath, bool removeExisting)
        {
            if (!Properties.ContainsKey(name))
                throw new ArgumentException("There is no file attached with the given name:" + name);

            if (File.Exists(targetFilePath))
            {
                if (removeExisting)
                    File.Delete(targetFilePath);
                else
                    throw new ArgumentException("The file already exists and removeExisting is set to false. Path:" +
                                                targetFilePath);
            }
            if (!File.Exists(targetFilePath))
            {
                try
                {
                    using (var writer = new BinaryWriter(File.OpenWrite(targetFilePath)))
                    {
                        byte[] decoded = Convert.FromBase64String((string)Properties[name]);
                        writer.Write(decoded);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("Error in saving the attached file", ex);
                    throw;
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="message">The message.</param>
        /// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
        new public static VirastyarLogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message)
        {
            return new VirastyarLogEventInfo(logLevel, loggerName, null, message, null);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
        new public static VirastyarLogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, object message)
        {
            return new VirastyarLogEventInfo(logLevel, loggerName, formatProvider, "{0}", new object[] { message });
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
        new public static VirastyarLogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message, Exception exception)
        {
            return new VirastyarLogEventInfo(logLevel, loggerName, null, message, null, exception);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
        new public static VirastyarLogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
        {
            return new VirastyarLogEventInfo(logLevel, loggerName, formatProvider, message, parameters);
        }

        #endregion

        #endregion
    }

    public class CDataCustomSerializer : ICustomSerializer<string>, ICustomDeserializer<string>
    {
        public string DeserializeFromElement(XElement element)
        {
            foreach (var node in element.Nodes())
            {
                if (node is XCData)
                {
                    return (node as XCData).Value;
                }
            }

            return "NotFound";
        }

        public void SerializeToElement(string objectToSerialize, System.Xml.Linq.XElement elemToFill)
        {
            if (objectToSerialize != null)
                elemToFill.Add(new XCData(objectToSerialize));
            else
                elemToFill.Add("");
        }

        #region Not Used

        public void SerializeToAttribute(string objectToSerialize, System.Xml.Linq.XAttribute attrToFill)
        {
        }

        public string SerializeToValue(string objectToSerialize)
        {
            return "NotImplemented";
        }

        public string DeserializeFromAttribute(XAttribute attrib)
        {
            return "NotImplemented";
        }


        public string DeserializeFromValue(string value)
        {
            return "NotImplemented";
        }
        #endregion
    }
}