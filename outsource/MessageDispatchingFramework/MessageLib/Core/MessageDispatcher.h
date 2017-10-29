#ifndef SERVICECENTER_H
#define SERVICECENTER_H

#include <map>
#include <vector>

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
   void registerMessageHandler(MessageHandler* handler, unsigned int messageType);
   void unregisterMessageHandler(MessageHandler* handler, unsigned int messageType);
   void postMessageToHandlers(Message* msg);
   MessageDispatcher(){}

   MapOfMessageHandler _handlersMap;
};
}
#endif // SERVICECENTER_H
