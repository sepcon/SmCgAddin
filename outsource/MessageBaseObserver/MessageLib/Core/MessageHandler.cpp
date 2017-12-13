#include "MessageHandler.h"
#include "MessageDispatcher.h"
#include "Message.h"
#include "Util/CommonLogger.h"

#include <algorithm>

namespace MsgLib
{

MessageHandler::MessageHandler()
{

}

void MessageHandler::registerToMessage(const char* msgType)
{
   if(std::find(_registeredMessgeTypes.begin(), _registeredMessgeTypes.end(), msgType) == _registeredMessgeTypes.end())
   {
      _registeredMessgeTypes.push_back(msgType);
      MessageDispatcher::getInstance()->registerHandler(this, msgType);
   }
}

/**
 * After calling this function, MessageHandler will no longer handle the message with type msgType
 * @param msgType: type of message
 */
void MessageHandler::unregisterToMessage(const char* msgType)
{
   _registeredMessgeTypes.erase(
            std::remove(_registeredMessgeTypes.begin(), _registeredMessgeTypes.end(), msgType)
            , _registeredMessgeTypes.end());
   MessageDispatcher::getInstance()->unregisterHandler(this, msgType);
}

/**
 * Delegate handling message to subclass
 * @param msg
 */
void MessageHandler::handle(Message *msg)
{
   if(!msg)
   {
      MSG_ERROR(className() << " does not handle Message Null!!!");
   }
   else
   {
      MSG_LOG( className() << " is starting handling message: " << msg->type());
      handleMessage(msg);
   }
}


MessageHandler::~MessageHandler()
{
   for(size_t i = 0; i < _registeredMessgeTypes.size(); ++i)
   {
      MessageDispatcher::getInstance()->unregisterHandler(this, _registeredMessgeTypes[i]);
   }
}

}
