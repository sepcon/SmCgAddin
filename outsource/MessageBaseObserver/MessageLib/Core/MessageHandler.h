#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <vector>

namespace MsgLib {

class Message;
class MessageHandler
{
public:
   MessageHandler();
   void registerToMessage(unsigned int msgType);
   void unregisterToMessage(unsigned int msgType);
   void handle(Message* msg);
   virtual const char* className() { return "MessageHandler"; }

   virtual ~MessageHandler();

protected:
   virtual void handleMessage(Message* msg) = 0;

private:
   std::vector<unsigned int> _registeredMessgeTypes;
};
}

#endif // MESSAGEHANDLER_H
