#ifndef MESSAGEHANDLERMACROS_H
#define MESSAGEHANDLERMACROS_H

#include "Core/MessageHandler.h"
#include "MessageInterfaces.h"
#include <string.h>

#define REGISTER_HANDLING_MESSAGE(MessageName) \
   registerToMessage(MessageName::sType())

#define HANDLE_MESSAGE_START(HandlerName) \
   public: \
   const char* className() { return #HandlerName; } \
   protected: \
   void handleMessage(MsgLib::Message* msgPtr)  \
   { \
      if(msgPtr) \
      { \
         if (false) { \
         } //nothing to handle

#define HANDLE_MESSAGE(MessageName) \
         else if(strcmp(msgPtr->type(), #MessageName) == 0) { \
            handleMessage(DOWN_CAST(MessageName, msgPtr)); \
         }

#define HANDLE_MESSAGE_END \
      } \
   }


#endif // MESSAGEHANDLERMACROS_H
