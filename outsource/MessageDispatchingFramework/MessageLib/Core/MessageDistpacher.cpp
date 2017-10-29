#include "MessageDispatcher.h"
#include "Message.h"
#include "MessageHandler.h"
#include "Util/CommonLogger.h"
#include "algorithm"

namespace MsgLib
{
MessageDispatcher *MessageDispatcher::getInstance()
{
   static MessageDispatcher sMsgRouter;
   return &sMsgRouter;
}

void MessageDispatcher::registerMessageHandler(MessageHandler *handler, unsigned int messageType)
{
   if(!handler)
   {
      MSG_ERROR("TRYING TO REGISTER NULL HANDLER FOR MESSAGE TYEP: " << messageType);
   }
   else
   {
      MSG_LOG(handler->handlerName() << " registers to handle message: " << messageType);
      _handlersMap[messageType].push_back(handler);
   }
}

void MessageDispatcher::unregisterMessageHandler(MessageHandler *handler, unsigned int messageType)
{
   MapOfMessageHandler::iterator it = _handlersMap.find(messageType);
   if(it != _handlersMap.end())
   {
      std::vector<MessageHandler* >& handlerList = it->second;
      handlerList.erase(std::remove(handlerList.begin(), handlerList.end(), handler), handlerList.end());
   }
   else
   {
      MSG_ERROR("THERE'S NO HANDLER NAME: " << handler->handlerName() << " register for handling message type: " << messageType);
   }
}

void MessageDispatcher::postMessageToHandlers(Message *msg)
{
   std::vector<MessageHandler* >& handlerList = _handlersMap[msg->type()];
   if(handlerList.empty())
   {
      MSG_ERROR("ERROR: there's no handler for Message Type: " << msg->type());
   }
   else
   {
      for(size_t i = 0; i < handlerList.size(); ++i)
      {
         handlerList[i]->handle(msg);
      }
   }
}

}
