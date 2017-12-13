#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <vector>

namespace MsgLib {

class Message;
class MessageHandler
{
public:
   MessageHandler();
   void registerToMessage(const char* msgType);
   void unregisterToMessage(const char* msgType);
   void handle(Message* msg);
   virtual const char* className() { return "MessageHandler"; }

   virtual ~MessageHandler();

protected:
   virtual void handleMessage(Message* msg) = 0;

private:
   std::vector<const char*> _registeredMessgeTypes;
};
} // namespace MsgLib

#endif // MESSAGEHANDLER_H
