#ifndef MESSAGE_H
#define MESSAGE_H

#include <map>
namespace MsgLib
{
class Message
{
public:
   Message() {}
   virtual ~Message() {}
   virtual unsigned int type() = 0;
   virtual const char* className() { return "Message"; }
   void post();
};
}

#endif // MESSAGE_H
