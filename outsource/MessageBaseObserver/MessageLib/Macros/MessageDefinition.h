#ifndef MESSAGEMACROSPRV_H
#define MESSAGEMACROSPRV_H

#include "Util/Macros.h"
#include "Core/Message.h"
#include "CXX.h"

#define MSG_DEF_DECLARE__(type, order) type _var_##order;
#define MSG_DEF_DECLARE_(...) CALL_MACRO_X_FOR_EACH(MSG_DEF_DECLARE__, MSG_DEF_DECLARE__, __VA_ARGS__)
//MSG_DEF_DECLARE_(int, int)

#define MSG_DEF_GET_FUNC_PARAM_LAST__(type, order) type& var_##order
#define MSG_DEF_GET_FUNC_PARAM__(type, order) MSG_DEF_GET_FUNC_PARAM_LAST__(type, order), //(type, order) type var_##order,
#define MSG_DEF_GET_FUNC_PARAMS_(...)  CALL_MACRO_X_FOR_EACH(MSG_DEF_GET_FUNC_PARAM__, MSG_DEF_GET_FUNC_PARAM_LAST__, __VA_ARGS__)
//MSG_DEF_GET_FUNC_PARAMS_(int, int)

#define MSG_DEF_SET_FUNC_PARAM_LAST__(type, order) const MSG_DEF_GET_FUNC_PARAM_LAST__(type, order)
#define MSG_DEF_SET_FUNC_PARAM__(type, order) MSG_DEF_SET_FUNC_PARAM_LAST__(type, order), //(type, order) type var_##order,
#define MSG_DEF_SET_FUNC_PARAMS_(...)  CALL_MACRO_X_FOR_EACH(MSG_DEF_SET_FUNC_PARAM__, MSG_DEF_SET_FUNC_PARAM_LAST__, __VA_ARGS__)
//MSG_DEF_SET_FUNC_PARAMS_(int, int)

#define MSG_DEF_GET_DATA_FUNC_STATEMENT__(type, order) var_##order = _var_##order;
#define MSG_DEF_GET_DATA_FUNC_BODY_(...) CALL_MACRO_X_FOR_EACH(MSG_DEF_GET_DATA_FUNC_STATEMENT__, MSG_DEF_GET_DATA_FUNC_STATEMENT__, __VA_ARGS__)
//MSG_DEF_GET_DATA_FUNC_BODY_(int, int)

#define MSG_DEF_SET_DATA_FUNC_STATEMENT__(type, order) _var_##order = var_##order;
#define MSG_DEF_SET_DATA_FUNC_BODY_(...) CALL_MACRO_X_FOR_EACH(MSG_DEF_SET_DATA_FUNC_STATEMENT__, MSG_DEF_SET_DATA_FUNC_STATEMENT__, __VA_ARGS__)
//MSG_DEF_SET_DATA_FUNC_BODY_(int, int)

#define MSG_DEF_CONSTRUCTOR_INITIALIZE_LAST__(type, order) _var_##order(var_##order)
#define MSG_DEF_CONSTRUCTOR_INITIALIZE__(type, order) MSG_DEF_CONSTRUCTOR_INITIALIZE_LAST__(type, order),
#define MSG_DEF_CONSTRUCTOR_INITIALIZE_LIST_(...) CALL_MACRO_X_FOR_EACH(MSG_DEF_CONSTRUCTOR_INITIALIZE__, MSG_DEF_CONSTRUCTOR_INITIALIZE_LAST__, __VA_ARGS__)
//MSG_DEF_CONSTRUCTOR_INITIALIZE_LIST_(int, int)

#define MSG_DEF_ENUM_VALUE(className) CONCAT_(ENUM_VALUE_PREFIX_, className)
#define CONCAT_(first, second) CONCAT__(first, second)
#define CONCAT__(first, second) first##second
#define ENUM_NAME_PREFIX_ EnMsg
#define ENUM_VALUE_PREFIX_ en

#define MSG_DEF_DECLARE_MESSAGE_WITH_PARAMS_(MessageName, ...) \
   class MessageName : public MsgLib::Message \
   { \
     public: \
      MessageName() {} \
      MessageName(MSG_DEF_SET_FUNC_PARAMS_(__VA_ARGS__)) \
         : MSG_DEF_CONSTRUCTOR_INITIALIZE_LIST_(__VA_ARGS__) {} \
      \
      const char* type() MSG_OVERRIDE; \
      static const char* sType(); \
      \
      void getData( MSG_DEF_GET_FUNC_PARAMS_(__VA_ARGS__)) const \
      { \
         MSG_DEF_GET_DATA_FUNC_BODY_(__VA_ARGS__) \
      } \
      MSG_DEF_DECLARE_(__VA_ARGS__) \
   };

#define MSG_DEF_DECLARE_MESSAGE_NO_PARAM_(MessageName) \
   class MessageName : public MsgLib::Message \
   { \
     public: \
      MessageName() {} \
      const char* type() MSG_OVERRIDE; \
      static const char* sType(); \
   };

#define MSG_DEF_DEFINE_MESSAGE_FUNCTIONS_(MessageName) \
   const char* MessageName::type() { return MessageName::sType(); } \
   const char* MessageName::sType() { return #MessageName; }

#define MSG_DEF_POST_MESSAGE_(MessageName, ...) { \
      MsgLib::Message* msg = MSG_DEF_CREATE_NEW_MESSAGE_(MessageName, ##__VA_ARGS__); \
      msg->post(); \
      delete msg; msg = 0; \
   }
#define MSG_DEF_CREATE_NEW_MESSAGE_(MessageName, ...) (new MessageName(__VA_ARGS__))

#define MSG_DEF_GET_MESSAGE_DATA_KNOWN_TYPE_(msgPtr, ...) \
   if(msgPtr) \
   { \
      msgPtr->getData(__VA_ARGS__); \
   }

#define MSG_DEF_GET_MESSAGE_DATA_(msgPtr, MessageName, ...) \
   MSG_DEF_GET_MESSAGE_DATA_KNOWN_TYPE_(DOWN_CAST(const MessageName, msgPtr), ##__VA_ARGS__)



#define _DCMS_PRMS_ MSG_DEF_DECLARE_MESSAGE_WITH_PARAMS_
#define _DCMS_NPRMS_  MSG_DEF_DECLARE_MESSAGE_NO_PARAM_
#define MSG_DEF_DECLARE_MESSAGE__(MessageName, ...) \
    _GET_NTH_ARG("ignored", ##__VA_ARGS__, \
   _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, \
   _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_PRMS_, _DCMS_NPRMS_)(MessageName, ##__VA_ARGS__)



#define MSG_DEF_ENUM_START_(Enum) enum CONCAT_(ENUM_NAME_PREFIX_, Enum) {enReservedvalue = 0,
#define MSG_DEF_ENUM_END_(Enum)  /*EnInvalidMessage*/};
#define MSG_DEF_DELACRE_ENUM_(enumValue)  CONCAT_(ENUM_VALUE_PREFIX_, enumValue),

#define MSG_DEF_NAMESPACE_START_(NameSpace) namespace NameSpace {
#define MSG_DEF_NAMESPACE_END_(NameSpace)  };
#define MSG_DEF_DECLARE_MESSAGE_(MessageName, ...) MSG_DEF_DECLARE_MESSAGE__(MessageName, ##__VA_ARGS__)

#define NAMESPACE_START_(NameSpace) MSG_DEF_NAMESPACE_START_ (NameSpace)
#define NAMESPACE_END_(NameSpace) MSG_DEF_NAMESPACE_END_(NameSpace)
#ifdef MSG_DEF_MESSAGE_IMPLEMENTATION
   #define DECLARE_MESSAGE_(MessageName, ...) \
   MSG_DEF_DECLARE_MESSAGE_(MessageName, ##__VA_ARGS__) \
   MSG_DEF_DEFINE_MESSAGE_FUNCTIONS_(MessageName)
#else
   #define DECLARE_MESSAGE_(MessageName, ...) MSG_DEF_DECLARE_MESSAGE_(MessageName, ##__VA_ARGS__)
#endif //MSG_DEF_MESSAGE_IMPLEMENTATION
#endif // MESSAGEMACROSPRV_H

////-->NON INCLUDE GUARD SECTION<--
//#ifdef NAMESPACE_START_
//   #undef NAMESPACE_START_
//#endif
//#ifdef NAMESPACE_END_
//   #undef NAMESPACE_END_
//#endif
//#ifdef DECLARE_MESSAGE_
//   #undef DECLARE_MESSAGE_
//#endif


//#ifdef ALLOW_CREATE_MESSAGES_ID_ENUM // generating enum for messages
//#undef ALLOW_CREATE_MESSAGES_ID_ENUM
//   #ifndef INCLUDE_ORDER
//      #define INCLUDE_ORDER 1
//      #define NAMESPACE_START_(NameSpace) MSG_DEF_ENUM_START_ (NameSpace)
//      #define NAMESPACE_END_(NameSpace) MSG_DEF_ENUM_END_(NameSpace)
//      #define DECLARE_MESSAGE_(MessageName, ...) MSG_DEF_DELACRE_ENUM_(MessageName)
//   #else
//      #error "wrong include order"
//   #endif
//#else
//#ifdef ALLOW_CREATE_MESSAGES_CLASS // generating Message classes
//#undef ALLOW_CREATE_MESSAGES_CLASS
//   #if !(defined INCLUDE_ORDER) || (INCLUDE_ORDER != 1)
//      #error "wrong include order, must be include with define ALLOW_CREATE_MESSAGES_ID_ENUM first"
//   #else
//      #undef INCLUDE_ORDER
//      #define INCLUDE_ORDER 2
//      #define NAMESPACE_START_(NameSpace) MSG_DEF_NAMESPACE_START_ (NameSpace)
//      #define NAMESPACE_END_(NameSpace) MSG_DEF_NAMESPACE_END_(NameSpace)
//      #define DECLARE_MESSAGE_(MessageName, ...) MSG_DEF_DECLARE_MESSAGE_(MessageName, ##__VA_ARGS__)
//   #endif

//#endif
//#endif
////<--NON INCLUDE GUARD SECTION-->
