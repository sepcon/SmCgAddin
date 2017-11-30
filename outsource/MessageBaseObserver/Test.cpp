#include <iostream>

#include "MessageMacrosUtil/MsgHandlerMcrInterfaces.h"
//#include "MessageHandler.h"
#include "MessagesAll.h"

using namespace std;
using namespace ::MyMessage;


class MessageCreator
{
public:
   void sendMessage()
   {
      std::cout << "SENDDING NEW SERIES OF MESSAGES" << std::endl;
      int i = 0;
      VectorOfChange p;
      std::vector<int> vec = {1, 2, 3, 4, 5};
      POST_MESSAGE(HelloWorldMessage);
      POST_MESSAGE(HelloWorldWithOnePersonMessage, "Noel");
      POST_MESSAGE(HelloWorldWithTwoParams, "Noel", "Child");
      POST_MESSAGE(HelloWorldWithThreeParams, 1, "Noel", 100.0);
      POST_MESSAGE(VectorOfChange, vec);
      POST_MESSAGE(PointerChange, &vec);
      POST_MESSAGE(OtherMessageChange, &p);
      POST_MESSAGE(TenParamMessage, &i, 1, 2,3,4,5,6,7,8,9);
      POST_MESSAGE(HelloWorldWithOnePersonMessage, "nguyen van con");
   }
};

class HelloWorldMessageHandler : public MsgLib::MessageHandler
{
public:
   HelloWorldMessageHandler()
   {
      REGISTER_HANDLING_MESSAGE(HelloWorldMessage);
      REGISTER_HANDLING_MESSAGE(HelloWorldWithOnePersonMessage);
      REGISTER_HANDLING_MESSAGE(HelloWorldWithThreeParams);
      REGISTER_HANDLING_MESSAGE(VectorOfChange);
      REGISTER_HANDLING_MESSAGE(OtherMessageChange);
      REGISTER_HANDLING_MESSAGE(TenParamMessage);
   }

   void handleMessage(TenParamMessage* msg)
   {
      Pointer<int> pa1;
      int a2;
      int a3;
      int a4;
      int a5;
      int a6;
      int a7;
      int a8;
      int a9;
      int a10;
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, pa1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
      if(pa1)
      {
         cout << *pa1 << a2 << a3 << a4 << a5 << a6 << a7 << a8 << a9 << a10 << endl;
      }
   }
   void handleMessage(HelloWorldMessage* /*msg*/)
   {
      cout << "hello world" << endl;
   }

   void handleMessage(HelloWorldWithOnePersonMessage* msg)
   {
      std::string name;
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, name);
      cout << "HELLO " << name << endl;
   }
   void handleMessage(HelloWorldWithThreeParams *msg)
   {
      int id;
      double value;
      std::string name;
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, id, name, value);
      cout << "HELLO " << name << " WITH ID: " << id << " VALUE = " << value << endl;
   }

   void handleMessage(VectorOfChange* msg)
   {
      std::vector<int> vec;
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, vec);
      {
         for(size_t i = 0; i < vec.size(); ++i)
         {
            cout << (vec)[i] << endl;
         }
      }
   }

   void handleMessage(OtherMessageChange* msg)
   {
      Pointer<MsgLib::Message> p;
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, p);
      if(p)
      {
         cout << "Message " << p->type() << " changed!" << endl;
      }
   }

   HANDLE_MESSAGE_START(HelloWorldMessageHandler)
   HANDLE_MESSAGE(HelloWorldMessage)
   HANDLE_MESSAGE(HelloWorldWithOnePersonMessage)
   HANDLE_MESSAGE(HelloWorldWithThreeParams)
   HANDLE_MESSAGE(VectorOfChange)
   HANDLE_MESSAGE(OtherMessageChange)
   HANDLE_MESSAGE(TenParamMessage)
   HANDLE_MESSAGE_END

};

#include <unistd.h>

int main()
{
   MessageCreator mc;
//   std::shared_ptr<HelloWorldMessageHandler> pH(new HelloWorldMessageHandler);
   mc.sendMessage();
   while(true)
   {
       mc.sendMessage();
       sleep(1);
   }
   
   return 0;
}

