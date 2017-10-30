#include "MsgMacros.h"
#include "Core/Message.h"

//-->NON INCLUDE GUARD SECTION<--

/***
 * To start defining message, just create a MessageDefine.h header file,
 * Then follow the template:
 *
 *    NAMESPACE_START(NameSpace)      //Namespace is obligated to start declaring a message class
 *       DECLARE_MESSAGE(MessageName1, argType1, argType2, ...)
 *       DECLARE_MESSAGE(MessageName2, argType1, argType2, ...)
 *    NAMESPACE_END(NameSpace)        // close the namespace NameSpace
 *
 * To generate MessageClass and MessageId for using with MessageLib -> create a MessageAll.h header file
 * Then follow the template:
 *
 * #define ALLOW_CREATE_MESSAGES_ID_ENUM   // --> to generate the enum of message ID
 *    #include MessageDefine.h
 * #define ALLOW_CREATE_MESSAGES_CLASS     // --> to generate class definition
 *    #include "MessageDefine.h"
 *
 * Result of preprocess step with definition like above:
 * enum EnNameSpace {
 *          enReservedvalue = 0,
 *          enMessageName1,
 *          enMessageName2,
 *       };
 *
 * namespace NameSpace {
 * class MessageName1 {
 * public:
 *    MessageName1(const argType1& var_1, const argType2& var_2, ...) : _var_1(var_1), _var_2(var_2), ... {}
 *    unsigned int type() { return enMessageName1; )
 *    void getType(argType1& var_1, argType2& var_2, ...) { var_1 = _var_1; var_2 = _var_2; ...}
 *
 * private:
 *    argType1 _var_1;
 *    argType2 _var_2;
 * };
 * class MessageName2 {
 * .........definition of MessageName2.....
 * };
 * } // NameSpace
 * */

#define NAMESPACE_START(NameSpace) NAMESPACE_START_(NameSpace)
#define NAMESPACE_END(NameSpace) NAMESPACE_END_(NameSpace)
/***
 * Declare new class message with type(enumtype) and list of data message conveys
 * E.g: DECLARE_MESSAGE(enListDataChanged, int, std::vector<std::string>)
 **/
#define DECLARE_MESSAGE(MessageClassName, ...) DECLARE_MESSAGE_(MessageClassName, ##__VA_ARGS__)



//<--NON INCLUDE GUARD SECTION-->

#ifndef MESSAGEMACROS_H
#define MESSAGEMACROS_H

/***
  * Create and post message with MessageClassName, and list of data that msg conveys
  * [MessageClassName]: enum Message Type
  * [__VA_ARGS__]: list of data that msg conveys
  * E.g: std::vector<std::string> listData;
  * int listID = 1000;
  * POST_MESSAGE(enListDataChanged, listID, listData);
  **/
#define POST_MESSAGE(MessageClassName, ...) MSG_DEF_POST_MESSAGE_(MessageClassName, ##__VA_ARGS__)
/***
 * Get data from a known type message
 * CMS_enListDataChanged* msgPtr;
 * int listId; std::vector<std::string> listData;
 * E.g: GET_MESSAGE_DATA(msgPtr, listId, listData);
 **/
#define GET_MESSAGE_DATA_KNOWN_TYPE(msgPtr, ...) MSG_DEF_GET_MESSAGE_DATA_KNOWN_TYPE_(msgPtr, ##__VA_ARGS__)

/***
 * Get data from a message
 * MsgLib::Message* msgPtr;
 * int listId; std::vector<std::string> listData;
 * E.g: GET_MESSAGE_DATA(msgPtr, enListDataChanged, listId, listData);
 **/
#define GET_MESSAGE_DATA(msgPtr, MessageClassName, ...) MSG_DEF_GET_MESSAGE_DATA_(msgPtr, MessageClassName, ##__VA_ARGS__)


#endif // MESSAGEMACROS_H
