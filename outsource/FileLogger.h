#ifndef FILELOGGER_H
#define FILELOGGER_H

//logging incase early startup, TTFIS hasn't connected to target yet
#include <cstdlib>
#include <sstream>
#include <string>

#ifdef STARTUP_TRACES_ENABLE
#define SDS_LOGGER() (clStartupLogger::getInstance() << __func__)
#define SDS_CCA_FUNC_NAME(funcCode) (clStartupLogger::getInstance().getFunctionName(funcCode))
#define SDS_ENDLOG true
#define ENDLOG SDS_ENDLOG
#else
#define SDS_LOGGER()
#define SDS_CCA_FUNC_NAME(funcCode)
#define SDS_ENDLOG '\n'
#define ENDLOG SDS_ENDLOG
#endif //STARTUP_TRACES_ENABLE

namespace sds {
namespace adapter {

class ILogWriter;
class LogWriterFactory;
enum enLogWriterType
{
   enLWT_SystemEcho,
   enLWT_FileStream,
   enLWT_Null
};

class clStartupLogger
{
public:
   static clStartupLogger& getInstance();
   void setLogFile(const std::string& logFile);

   #define INSERT_OPERATOR_DCL(master, type) master& operator<<(type value);
   INSERT_OPERATOR_DCL(clStartupLogger, double)
   INSERT_OPERATOR_DCL(clStartupLogger, const std::string&)
   INSERT_OPERATOR_DCL(clStartupLogger, int)
   INSERT_OPERATOR_DCL(clStartupLogger, unsigned long)
   INSERT_OPERATOR_DCL(clStartupLogger, char)
   INSERT_OPERATOR_DCL(clStartupLogger, bool)
   void writeLog();


protected:
   clStartupLogger(const std::string& logFile = "/sdslog.log");
   std::string _logFile;
   ILogWriter* _logWriter;
   std::ostringstream _traceCreator;
};

class ILogWriter
{
public:
   virtual bool log(const std::string& /*trace*/) { return true; }
   virtual void setLogFile(const std::string& /*logFile*/) {}
   virtual bool clearLog() { return true; }
   virtual ~ILogWriter() {}
};

class LogWriterFactory
{
public:
   static ILogWriter* createLogWriter(enLogWriterType writerType);
};

}
}


#endif // FILELOGGER_H
