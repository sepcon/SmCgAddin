//Don't allow include guard for this file

#include "MessageMacrosUtil/MessageMcrInterfaces.h"
#include <memory>
#include <vector>
#include "Util/Pointer.h"

using namespace MsgLib;

NAMESPACE_START(MyMessage)
   DECLARE_MESSAGE(HelloWorldMessage)
   DECLARE_MESSAGE(HelloWorldWithOnePersonMessage, std::string /*personName*/)
   DECLARE_MESSAGE(HelloWorldWithTwoParams, std::string /*personName1*/, std::string /*personName2*/)
   DECLARE_MESSAGE(HelloWorldWithThreeParams, int /*index*/, std::string /*name*/, double /*Value*/)
   DECLARE_MESSAGE(VectorOfChange, std::vector<int> /*listOfIndex*/)
   DECLARE_MESSAGE(PointerChange, Pointer<std::vector<int> >)
   DECLARE_MESSAGE(OtherMessageChange, Pointer<MsgLib::Message>)
   DECLARE_MESSAGE(TenParamMessage, Pointer<int>, int, int, int, int, int, int, int, int, int)
NAMESPACE_END(MyMessage)

