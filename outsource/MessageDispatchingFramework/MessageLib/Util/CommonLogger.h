#ifndef COMMONLOGGER_H
#define COMMONLOGGER_H

#ifdef ENABLE_CONSOLE_LOG
#include <iostream>
/******************************************************************
 * Call logger with c++ oustput stream stype inside the parentheses
 * E.g: MSG_LOG("The output Message's value is: " << 100);
 * */
#define MSG_LOG(msg) CONSOLE_WRITE(std::cout, msg)
#define MSG_ERROR(msg) CONSOLE_WRITE(std::cerr, msg)
#define CONSOLE_WRITE(output, msg) output << msg << " --> FILE: " << __FILE__ << " -- LINE: " << __LINE__ << " -- FUNCTION: " << __PRETTY_FUNCTION__ << ": " << std::endl
#else
#define MSG_LOG(msg)
#define MSG_ERROR(msg)
#endif

#endif // COMMONLOGGER_H
