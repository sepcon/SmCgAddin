using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.Win32;
using System.Text;
using ARAddinDeploy;

namespace ARSourceGeneration
{

    public enum  LogLevel : int
    {
        INFO = 0,
        WARNING,
        ERROR,
        FATAL
    }
    

    public static class MessageLogger
    {
        public static void log(string message,
            LogLevel level = LogLevel.INFO,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_allowLogging)
            {
                if(level > LogLevel.INFO)
                {
                    _numberOfErrorMessage++;
                }

                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf("\\"));
                string strLogLevel = _logLevelsInString[(int)level];
                StringBuilder strBuilder = new StringBuilder(11 + message.Length + strLogLevel.Length + memberName.Length + sourceFilePath.Length + sourceLineNumber.ToString().Length);

                message = strBuilder.AppendFormat("[{0}:{1}:{2}][{3}]::[{4}]\n", fileName, sourceLineNumber, memberName, strLogLevel, message).ToString();
                
                LogWriter.Write(message);
            }
        }

        public static void log(string [] messageList,
            LogLevel level = LogLevel.INFO,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

            if(_allowLogging && messageList.Length > 0)
            {
                string oneMessage = messageList[0];

                for(int i = 1; i < messageList.Length; ++i)
                {
                    oneMessage += " --> " + messageList[i];
                }
                log(oneMessage, level, memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void logError(ErrorMessageFormatIndex errorCode, params string[] messageContents)
        {
            string errorFormat = FormatGetter.get(errorCode);
            string message = (new StringBuilder(errorFormat.Length + messageContents.Length*100).AppendFormat(errorFormat, messageContents)).ToString();
            log(message, LogLevel.ERROR);
        }

        public static void startLogging()
        {
            if (!_allowLogging)
            {
                clearOldLog();
                updateWriter();
                _allowLogging = true;
                _numberOfErrorMessage = 0;
            }
        }


        public static void endLogging()
        {
            if (_allowLogging)
            {
                if (LogWriter != null)
                {
                    LogWriter.Close();
                    LogWriter = null;
                }
                _allowLogging = false;
            }
        }
        private static void clearOldLog()
        {
            if (File.Exists(LogFile))
            {
                if(LogWriter != null)
                {
                    LogWriter.Close();
                }
                FileStream fStream = File.Open(LogFile, FileMode.Truncate);
                fStream.Close();
                LogWriter = null;
            }
        }
        private static void updateWriter()
        {
            if (File.Exists(LogFile))
            {
                if (LogWriter == null)
                {
                    LogWriter = File.AppendText(LogFile);
                }
            }
            else
            {
                LogWriter = new StreamWriter(File.Create(LogFile), System.Text.Encoding.UTF8);
            }
        }


        private static StreamWriter LogWriter
        {
            get
            { 
                return _logWriter; 
            }
            set { _logWriter = value; if(_logWriter != null) _logWriter.AutoFlush = true; }
        }

        public static string LogFile
        {
            get
            {
                if (_logFile == "")
                {
                    _logFile = DeployInfo.getInstallationPath(DeployInfo.InstallationInfo.LogPathKey) + @"\log.txt";
                }
                return MessageLogger._logFile; 
            }
            set { MessageLogger._logFile = value; }
        }
        public static int NumberOfErrorMessages
        {
            get { return MessageLogger._numberOfErrorMessage; }
        }

        //private static string _messages = "";
        private static string[] _logLevelsInString = { "INFO", "WARNING", "ERROR", "FATAL" };
        private static string _logFile = "";
        private static StreamWriter _logWriter;
        private static bool _allowLogging = false;
        private static int _numberOfErrorMessage = 0;

        
    }

    
    public static class ErrorLogger
    {
        public static void log(Error.ErrorType type, Error.ErrorLevel level, params object[] listEAObjects)
        {

        }

        public static string getLog()
        {
            List<Error> listWarning = _listErrorMessage.FindAll(err => err.Level == Error.ErrorLevel.Warning);
            List<Error> listError = _listErrorMessage.FindAll(err => err.Level == Error.ErrorLevel.Error);
            string errorsMessageFmt = FormatGetter.get(ErrorMessageFormatIndex.GeneratingCodeSuccessful);
            string errors = "";
            string warnings = "";
            foreach(var e in listError)
            {
                errors += e.message() + ConstantValues.NEW_LINE;
            }
            foreach(var w in listWarning)
            {
                warnings += w.message() + ConstantValues.NEW_LINE; 
            }

            return string.Format(errorsMessageFmt, listError.Count, listWarning.Count, errors, warnings);
        }


        public static Error createError(Error.ErrorType type, Error.ErrorLevel level, params object[] listEAObjects)
        {
            Error e;
            switch (type)
            {
                case Error.ErrorType.ConnectorError:
                    e = new ConnectorError(type, level, listEAObjects);
                    break;
                default:
                    e = null;
                    break;
            }

            return e;
        }

        private static List<Error> _listErrorMessage = new List<Error>();
    }


    public abstract class Error
    {
        public enum ErrorType : int
        {
            ConnectorError,

        }

        public enum ErrorLevel
        {
            Warning,
            Error
        }

        public Error(ErrorType type, ErrorLevel level, params object[] listEAObjects)
        {
            Type = type;
            Level = level;
            _listErrorArgs = getListErrorArgs(listEAObjects);
        }
       
        public virtual string message()
        {
            return string.Format(FormatGetter.get(FormatIndex), _listErrorArgs);
        }

        protected abstract string[] getListErrorArgs(params object[] listEAObjects);


        public ErrorType Type { get; set; }
        public ErrorLevel Level { get; set; }

        public abstract ErrorMessageFormatIndex FormatIndex {get;}
        
        private string[] _listErrorArgs;
        
    }

    class ConnectorError : Error
    {
        public ConnectorError(ErrorType type, ErrorLevel level, params object[] listEAObjects) :
            base(type, level, listEAObjects)
        {

        }

        protected override string[] getListErrorArgs(params object[] listEAObjects)
        {
            return new string[] { };
        }
        public override ErrorMessageFormatIndex FormatIndex { get { return ErrorMessageFormatIndex.ConnectorError; } }
    }
}