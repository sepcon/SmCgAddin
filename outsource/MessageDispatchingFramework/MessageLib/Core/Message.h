#ifndef MESSAGE_H
#define MESSAGE_H

namespace MsgLib
{
class Message
{
public:
   Message() {}
   virtual ~Message() {}
   virtual unsigned int type() = 0;
   void post();
};
}

#endif // MESSAGE_H
