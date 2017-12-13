#ifndef SERVICECENTER_H
#define SERVICECENTER_H

#include <map>
#include <vector>

#include "MessageHandler.h"

namespace MsgLib
{
class MessageHandler;
class Message;
class MessageDispatcher
{
public:
   typedef  const char* MessageType;
   typedef std::map<MessageType, std::vector<MessageHandler*> > MapOfMessageHandler;
   friend class MessageHandler;
   friend class Message;

private:
   static MessageDispatcher* getInstance();
   void registerHandler(MessageHandler* handler, MessageType messageType);
   void unregisterHandler(MessageHandler* handler, MessageType messageType);
   void dispatch(Message* msg);
   MessageDispatcher(){}

   MapOfMessageHandler _handlersMap;
};
}
#endif // SERVICECENTER_H
