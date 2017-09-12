#include "FileLogger.h"
///SDS_LOGGER

#include <map>
#include <string>


std::map<unsigned int, std::string> gFunctionMap;

static bool executeCommand(const std::string& cmd)
{
   return !system(cmd.c_str());
}

namespace sds {
namespace adapter {



clStartupLogger &clStartupLogger::getInstance()
{
//   "/opt/bosch/sds/bin/sds_adapter.log"
   static clStartupLogger sInstance("hello.log");
   return sInstance;
}

#define INSERT_OPERATOR_DEF(master, type)\
   master &master::operator<<(type value) \
   { \
      _traceCreator << " " <<  value; \
      return *this; \
   }
INSERT_OPERATOR_DEF(clStartupLogger, double)
INSERT_OPERATOR_DEF(clStartupLogger, const std::string &)
INSERT_OPERATOR_DEF(clStartupLogger, const char*)
INSERT_OPERATOR_DEF(clStartupLogger, int)
INSERT_OPERATOR_DEF(clStartupLogger, unsigned long)
INSERT_OPERATOR_DEF(clStartupLogger, char)
//for trigger writing log to destination output
clStartupLogger& clStartupLogger::operator <<(bool b)
{
   if(b) { writeLog(); }

   return *this;
}



clStartupLogger::clStartupLogger(const std::string &logFile) : _logFile(logFile)
{
   system("rwrfs"); //make file system writable
   _logWriter = LogWriterFactory::createLogWriter(enLWT_SystemEcho);
   _logWriter->setLogFile(_logFile);
   _logWriter->clearLog();

   gFunctionMap[0x100] = "COMMONSTARTSESSION";
   gFunctionMap[0x101] = "COMMONSTOPSESSION";
   gFunctionMap[0x102] = "COMMONSHOWDIALOG";
   gFunctionMap[0x103] = "COMMONSELECTLISTELEMENT";
   gFunctionMap[0x104] = "COMMONGETLISTELEMENT";
   gFunctionMap[0x105] = "COMMONSETACTIVEAPPLICATION";
   gFunctionMap[0x106] = "COMMONSETAVAILABLESPEAKERS";
   gFunctionMap[0x107] = "COMMONSETAVAILABLEUSERWORDS";
   gFunctionMap[0x108] = "COMMONINTERACTIONLOGGER";
   gFunctionMap[0x109] = "COMMONSETTINGS";
   gFunctionMap[0x10A] = "COMMONGETLISTINFO";
   gFunctionMap[0x200] = "TUNERSELECTSTATION";
   gFunctionMap[0x201] = "TUNERSELECTBAND_MEMBANK";
   gFunctionMap[0x202] = "TUNERAUTOSTORE";
   gFunctionMap[0x203] = "TUNERSETSETTING";
   gFunctionMap[0x204] = "TUNERSTORESTATION";
   gFunctionMap[0x205] = "TUNERSHOWSTATIONLIST";
   gFunctionMap[0x206] = "TUNERGETLISTENTRIES";
   gFunctionMap[0x207] = "TUNERGETDATABASES";
   gFunctionMap[0x208] = "TUNERGETAMBIGUITYLIST";
   gFunctionMap[0x401] = "PHONESELECTPHONE";
   gFunctionMap[0x402] = "PHONESTARTPAIRING";
   gFunctionMap[0x403] = "PHONEDIALNUMBER";
   gFunctionMap[0x404] = "PHONEREDIALLASTNUMBER";
   gFunctionMap[0x405] = "PHONEGETCONTACTLISTENTRIES";
   gFunctionMap[0x406] = "PHONESTORENUMBER";
   gFunctionMap[0x407] = "PHONEDELETENUMBER";
   gFunctionMap[0x408] = "PHONESETPHONESETTING";
   gFunctionMap[0x409] = "PHONEDIALVOICEMAIL";
   gFunctionMap[0x40A] = "PHONEGETDATABASES";
   gFunctionMap[0x40B] = "PHONEDIALCONTACT";
   gFunctionMap[0x40C] = "PHONESHOWMENU";
   gFunctionMap[0x40D] = "PHONESENDDTMFDIGITS";
   gFunctionMap[0x40E] = "PHONESWITCHCALL";
   gFunctionMap[0x40F] = "PHONESETCONTACT";
   gFunctionMap[0x410] = "PHONEGETNUMBERINFO";
   gFunctionMap[0x501] = "MEDIAPLAY";
   gFunctionMap[0x502] = "MEDIAGETDATABASE";
   gFunctionMap[0x503] = "MEDIAGETAMBIGUITYLIST";
   gFunctionMap[0x504] = "MEDIAGETDEVICEINFO";
   gFunctionMap[0x601] = "NAVISTARTGUIDANCE";
   gFunctionMap[0x602] = "NAVISTOPGUIDANCE";
   gFunctionMap[0x603] = "NAVISETZOOMSETTING";
   gFunctionMap[0x604] = "NAVIGETROUTECRITERIA";
   gFunctionMap[0x605] = "NAVISETROUTECRITERIA";
   gFunctionMap[0x606] = "NAVISETNAVSETTING";
   gFunctionMap[0x607] = "NAVISHOWNAVMENU";
   gFunctionMap[0x608] = "NAVISETMAPMODE";
   gFunctionMap[0x609] = "NAVIGETMAPMODE";
   gFunctionMap[0x60A] = "NAVIGETCONTACTLISTENTRIES";
   gFunctionMap[0x60B] = "NAVISELECTDESTLISTENTRY";
   gFunctionMap[0x60C] = "NAVISTOREDESTINATION";
   gFunctionMap[0x60D] = "NAVIDELETEDESTINATION";
   gFunctionMap[0x60E] = "NAVINEWDESTINATION";
   gFunctionMap[0x60F] = "NAVIGETCURRENTDESTINATION";
   gFunctionMap[0x610] = "NAVISETDESTINATIONITEM";
   gFunctionMap[0x611] = "NAVIGETAMBIGUITYLIST";
   gFunctionMap[0x614] = "NAVISHOWTMCLIST";
   gFunctionMap[0x615] = "NAVISHOWPOILIST";
   gFunctionMap[0x616] = "NAVIGETPOICATEGORIES";
   gFunctionMap[0x617] = "NAVIGETTMCLISTENTRIES";
   gFunctionMap[0x618] = "NAVISELECTTMCLISTENTRY";
   gFunctionMap[0x619] = "NAVISETDESTINATIONCONTACT";
   gFunctionMap[0x61A] = "NAVISETDESTINATIONASWAYPOINT";
   gFunctionMap[0x61B] = "NAVIGETHOUSENUMBERRANGE";
   gFunctionMap[0x61C] = "NAVIGETWAYPOINTLISTINFO";
   gFunctionMap[0x61D] = "NAVISTARTLOCATIONSEARCH";
   gFunctionMap[0x701] = "WEATHERSETCONTACT";
   gFunctionMap[0x702] = "WEATHERSETLOCATION";
   gFunctionMap[0x801] = "TEXTMSGGETINFO";
   gFunctionMap[0x802] = "TEXTMSGCALLBACKSENDER";
   gFunctionMap[0x803] = "TEXTMSGSETCONTENT";
   gFunctionMap[0x804] = "TEXTMSGSETNUMBER";
   gFunctionMap[0x805] = "TEXTMSGSEND";
   gFunctionMap[0x806] = "TEXTMSGSELECTMESSAGE";
   gFunctionMap[0x901] = "CONTACTSGETAMBIGUITYLIST";
   gFunctionMap[0xA00] = "INFOSHOWMENU";
   gFunctionMap[0xA01] = "VDLGETDATABASES";
   gFunctionMap[0xA02] = "APPSLAUNCHAPPLICATION";
   gFunctionMap[0xA03] = "COMMONSETDYNACCESSINFO";
   gFunctionMap[0xA04] = "NAVIGETNEARBYSTATES";
   gFunctionMap[0xA05] = "COMMONGETHMIELEMENTDESCRIPTION";
   gFunctionMap[0xA06] = "NAVIREPEATINSTRUCTION";
   gFunctionMap[0xA07] = "TEXTMSGGETCONTENT";
   gFunctionMap[0xA08] = "COMMONGETHMILISTDESCRIPTION";
   gFunctionMap[0xA09] = "PHONEUPDATEPHONEBOOK";
   gFunctionMap[0xA0A] = "NAVIGETSEARCHLOCATION";
   gFunctionMap[0xA0B] = "COMMONSETSDSEVENT";
   gFunctionMap[0xA0C] = "TVSELECTSTATION";
   gFunctionMap[0xA0D] = "NAVDATAGETSTREETAVAILABILITY";
   gFunctionMap[0xA0E] = "NAVDATAGETSTRINGANDPHONEME";
   gFunctionMap[0xA0F] = "NAVDATAREGISTERDIRECTNDSUSE";
   gFunctionMap[0xA10] = "NAVDATAUNREGISTERDIRECTNDSUSE";
   gFunctionMap[0xA11] = "NAVDATAGETCOUNTRYSTATELIST";
   gFunctionMap[0xA12] = "PHONEGETPHONENUMBERFORMATTED";
   gFunctionMap[0xA13] = "COMMONRESTOREHMILIST";
   gFunctionMap[0xA14] = "INFOSHOWSERVICEADVISORY";
   gFunctionMap[0xA15] = "NAVISTARTDISTANCEDETOUR";
   gFunctionMap[0x0001] = "SDS_STATUS";
   gFunctionMap[0x0002] = "SDS_ACTIVESPEAKER";
   gFunctionMap[0x0003] = "COMMONACTIONREQUEST";
   gFunctionMap[0x0004] = "COMMONSTATUS";
   gFunctionMap[0x0006] = "MEDIASTATUS";
   gFunctionMap[0x0007] = "NAVISTATUS";
   gFunctionMap[0x0008] = "PHONESTATUS";
   gFunctionMap[0x0009] = "TUNERSTATUS";
   gFunctionMap[0x000A] = "WEATHERSTATUS";
   gFunctionMap[0x000B] = "NAVICURRENTCOUNTRYSTATE";
   gFunctionMap[0x000C] = "COMMONSETTINGSREQUEST";
   gFunctionMap[0x000D] = "TEXTMSGSTATUS";
   gFunctionMap[0x000E] = "VDLSTATUS";
   gFunctionMap[0x000F] = "CONNECTEDDEVICESTATUS";
   gFunctionMap[0x0010] = "NAVICURRENTNEIGHBORINGLOCATION";
   gFunctionMap[0x0011] = "SPECIALAPPSTATUS";
   gFunctionMap[0x0012] = "COMMONSDSCONFIGURATION_STAT";
   gFunctionMap[0x0013] = "COMMONSDSCONFIGURATION_DYNA";
   gFunctionMap[0x0014] = "COMMONCORESPEECHPARAMETERS";
   gFunctionMap[0x0015] = "NAVDATAACTIVEDATASET";
   gFunctionMap[0x0016] = "AUDIOECNR_ASR_MODE_STATUS";
   gFunctionMap[0x0017] = "AUDIO_ECNR_ENGINEPARAMETER";
   gFunctionMap[0x0018] = "INFOSERVICESTATUS";
   gFunctionMap[0xFFFF] = "INVALID_FUNCTIONID";
}

void clStartupLogger::writeLog()
{
   _logWriter->log(_traceCreator.str());
   _traceCreator.str("");
   _traceCreator.clear();
}

std::string getFunctionName(unsigned int functionCode)
{
   std::map<unsigned int, std::string>::iterator it = gFunctionMap.find(functionCode);
   if(it != gFunctionMap.end())
   {
      return it->second;
   }
   else
   {
      return "NOT A FUNCTION";
   }
}



/// START - Declaration: Private classes for log writing


class SystemEchoLogWriter : public ILogWriter
{

#define APPEND_TO_FILE_CMD(file, trace) "echo " + trace + " >> " + file
public:
   SystemEchoLogWriter();
   bool log(const std::string& trace);
   virtual void setLogFile(const std::string& logFile);
   virtual bool clearLog();

   ~SystemEchoLogWriter(){}

protected:
   bool _isFileWritable;
private:
   std::string _file;
};


class StreamFileWriter : public ILogWriter
{
public:
   bool log(const std::string& /*trace*/) { return true; }
   //tbd
};

class NoOutputLogWirter : public ILogWriter
{
public:
   bool log(const std::string& /*trace*/) { return true; }
};
/// END - Declaration: Private classes for log writing



/// START - DEFINITION: Private classes for log writing
SystemEchoLogWriter::SystemEchoLogWriter() : _isFileWritable(false)
{
}

bool SystemEchoLogWriter::log(const std::string &trace)
{
   if(_isFileWritable)
   {
      std::string command = APPEND_TO_FILE_CMD(_file, trace);
      system(command.c_str());
      return true;
   }
   else
   {
      return false;
   }
}

void SystemEchoLogWriter::setLogFile(const std::string &logFile)
{
   std::string cmd = "echo > " + logFile;
   if(executeCommand(cmd))
   {
      _file = logFile;
      _isFileWritable = true;
   }
}

bool SystemEchoLogWriter::clearLog()
{
   if(_isFileWritable)
   {
      std::string cmd = "echo > " + _file;
      executeCommand(cmd);
   }
   return true;
}
/// END - DEFINITION: Private classes for log writing


ILogWriter *LogWriterFactory::createLogWriter(enLogWriterType writerType)
{
   ILogWriter* pWriter = 0;

   switch (writerType) {
   case enLWT_SystemEcho:
      pWriter = new SystemEchoLogWriter;
      break;

   case enLWT_FileStream:
      pWriter = new StreamFileWriter;
      break;

   case enLWT_Null:
   default:
      pWriter = new NoOutputLogWirter;
      break;
   }

   return pWriter;
}

}
}
