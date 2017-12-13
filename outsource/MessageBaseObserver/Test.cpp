#include <iostream>

#include "Macros/MessageHandlerInterfaces.h"
//#include "MessageHandler.h"
#include "ProducerMessages.h"
#include "ConsummerMessages.h"
#include <unistd.h>

using namespace std;
using namespace ::Producer;
using namespace ::Consummer;


static class MessageCreator : MsgLib::MessageHandler
{
public:
   MessageCreator()
   {
      REGISTER_HANDLING_MESSAGE(::Consummer::MsgHandlingProduct);
   }
   void createProducts()
   {
      std::cout << "SENDDING NEW SERIES OF MESSAGES" << std::endl;

      for(int i = 0; i < 10; ++i)
      {
         _vec.push_back(i);
      }

      MsgLib::Message* msg = new MsgProductsCreated(makeRef(_vec));
      msg->post();
//      POST_MESSAGE(MsgProductsCreated, MsgLib::Pointer<std::vector<int> >(&_vec));
   }

   void handleMessage(MsgHandlingProduct* msg)
   {
      int product;
//      msg->getData(product);
      GET_MESSAGE_DATA_KNOWN_TYPE(msg, product)
      GET_MESSAGE_DATA(msg, MsgHandlingProduct, product)
      for(auto it = _vec.begin(); it != _vec.end(); ++it)
      {
         if(*it == product)
         {
            cout << "PRODUCER --------------- remove handled product: " << *it << endl;
            _vec.erase(it);
            break;
         }
      }
//      sleep(1);
      if(_vec.empty())
      {
//         sleep(2);
         createProducts();
      }
   }
   HANDLE_MESSAGE_START(MessageCreator)
   HANDLE_MESSAGE(MsgHandlingProduct)
   HANDLE_MESSAGE_END


   private:
      std::vector<int> _vec;
} gMsgCreator;

#include <signal.h>
void handleSigKill(int sig)
{
   if(SIGUSR1 == sig)
   {
      gMsgCreator.createProducts();
   }
}
class ProductConsummer : public MsgLib::MessageHandler
{
public:
   ProductConsummer()
   {
//      REGISTER_HANDLING_MESSAGE(MsgProductsCreated);
      REGISTER_HANDLING_MESSAGE(Producer::MsgProductsCreated);
   }

   HANDLE_MESSAGE_START(ProductConsummer)
   HANDLE_MESSAGE(MsgProductsCreated)
   HANDLE_MESSAGE_END

   void handleMessage(MsgProductsCreated* msg)
   {
      Reference<std::vector<int> > pProducts;

      msg->getData(pProducts);

//      if(pProducts)
//      {
         cout << "GET PRODUCTS: " << endl;
         while(!(*pProducts).empty())
         {
            cout << "CONSUMMER:----------------- handling product: " << (*pProducts).back() << endl;
            sleep(1);
            POST_MESSAGE(MsgHandlingProduct, (*pProducts).back());
         }
//      }
   }
};


int main()
{
   signal(SIGUSR1, handleSigKill);
   ProductConsummer consumer;
   MsgListTenOfProducts ten;
   cout << __cplusplus << endl;
   while(true);
}

