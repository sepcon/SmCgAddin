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

void MessageDispatcher::registerHandler(MessageHandler *handler, MessageType messageType)
{
   if(!handler)
   {
      MSG_ERROR("TRYING TO REGISTER NULL HANDLER FOR MESSAGE TYEP: " << messageType);
   }
   else
   {
      MSG_LOG(handler->className() << " REGISTERS TO HANDLE MESSAGE: " << messageType);
      _handlersMap[messageType].push_back(handler);
   }
}

void MessageDispatcher::unregisterHandler(MessageHandler *handler, MessageType messageType)
{
   MapOfMessageHandler::iterator it = _handlersMap.find(messageType);
   if(it != _handlersMap.end())
   {
      std::vector<MessageHandler* >& handlerList = it->second;
      handlerList.erase(std::remove(handlerList.begin(), handlerList.end(), handler), handlerList.end());
   }
   else
   {
      MSG_ERROR("THERE'S NO HANDLER NAME: " << handler->className() << " REGISTER FOR HANDLING MESSAGE TYPE: " << messageType);
   }
}

void MessageDispatcher::dispatch(Message *msg)
{
   std::vector<MessageHandler* >& handlerList = _handlersMap[msg->type()];
   if(handlerList.empty())
   {
      MSG_WARN("ERROR: THERE'S NO HANDLER FOR MESSAGE " << msg->type());
   }
   else
   {
      for(size_t i = 0; i < handlerList.size(); ++i)
      {
         handlerList[i]->handle(msg);
      }
   }
}

} // namespace MsgLib
