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
   typedef std::map<unsigned int, std::vector<MessageHandler*> > MapOfMessageHandler;
   friend class MessageHandler;
   friend class Message;

private:
   static MessageDispatcher* getInstance();
   void registerHandler(MessageHandler* handler, unsigned int messageType);
   void unregisterHandler(MessageHandler* handler, unsigned int messageType);
   void dispatch(Message* msg);
   MessageDispatcher(){}

   MapOfMessageHandler _handlersMap;
};
}
#endif // SERVICECENTER_H
