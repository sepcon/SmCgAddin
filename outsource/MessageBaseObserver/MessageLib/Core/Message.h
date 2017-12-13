#ifndef MESSAGE_H
#define MESSAGE_H

#include <map>
namespace MsgLib
{
class Message
{
public:
   Message() {}
   virtual ~Message();
   virtual const char* type() = 0;
   void post();
};
} // namespace MsgLib

#endif // MESSAGE_H
