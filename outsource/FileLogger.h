#ifndef FILELOGGER_H
#define FILELOGGER_H

//logging incase early startup, TTFIS hasn't connected to target yet
#include <sstream>
#include <string>

#ifdef STARTUP_TRACES_ENABLE
class clStartupLogger;
#define SDS_LOG(trace) \
   ((sds::adapter::logging::clStartupLogger::getInstance() \
   << "<<FILE: " << __FILE__ << ">>" \
   << "<<LINE: " << __LINE__ << ">>" \
   << "<<FUNC: "  <<__FUNCTION__ << ">>:: ") \
   << trace << SDS_ENDLOG)
#define SDS_ENDLOG sds::adapter::logging::clStartupLogger::getInstance()
#define SDS_CCA_FUNC_NAME(funcID) (sds::adapter::logging::getFunctionName(funcID))
#else
#define SDS_LOG(trace)
#define SDS_ENDLOG
#define SDS_CCA_FUNC_NAME(funcID)
#endif //STARTUP_TRACES_ENABLE

namespace sds {
namespace adapter {
namespace logging {

//need to move to other file accordingly
std::string getFunctionName(unsigned int funcID);

class ILogWriter;
class clStartupLogger
{
public:
   typedef clStartupLogger LoggerManipulatorType ;
   static clStartupLogger& getInstance();
   void setLogFile(const std::string& logFile);

   clStartupLogger& operator<<(double value);
   clStartupLogger& operator<<(long double value);
   clStartupLogger& operator<<(float value);
   clStartupLogger& operator<<(const std::string& value);
   clStartupLogger& operator<<(const char* value);
   clStartupLogger& operator<<(long value);
   clStartupLogger& operator<<(unsigned long value);
   clStartupLogger& operator<<(bool value);
   clStartupLogger& operator<<(short value);
   clStartupLogger& operator<<(unsigned short value);
   clStartupLogger& operator<<(int value);
   clStartupLogger& operator<<(unsigned int value);
   clStartupLogger& operator<<(char value);
   clStartupLogger& operator<<(const LoggerManipulatorType &value);



protected:
   clStartupLogger(const std::string& logFile);
   void writeLog();
   template<typename T>
   clStartupLogger& addToTrace(T value);

   std::string _logFile;
   ILogWriter* _logWriter;
   std::ostringstream _traceCreator;
};

} //logging
} //adapter
} //sds


#endif // FILELOGGER_H
