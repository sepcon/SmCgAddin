#ifndef MESSAGEHANDLERMACROS_H
#define MESSAGEHANDLERMACROS_H

#include "Core/MessageHandler.h"
#include "MessageMacros.h"


#define REGISTER_HANDLING_MESSAGE(MessageClassName) \
   registerToMessage(MSG_DEF_ENUM_VALUE(MessageClassName))

#define HANDLE_MESSAGE_START(MessageClassName) \
   public: \
   const char* handlerName() { return #MessageClassName; } \
   protected: \
   void handleMessage(MsgLib::Message* msgPtr)  \
   { \
      if(msgPtr) { \
         switch(msgPtr->type()) \
         {
#define HANDLE_MESSAGE(MessageClassName) \
      case MSG_DEF_ENUM_VALUE(MessageClassName): \
      handleMessage(DOWN_CAST(MessageClassName, msgPtr)); \
      break;

#define HANDLE_MESSAGE_END \
   default: \
      break; \
    } } }


#endif // MESSAGEHANDLERMACROS_H
