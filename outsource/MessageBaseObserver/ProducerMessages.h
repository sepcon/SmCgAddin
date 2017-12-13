#ifndef PRODUCERMESSAGES_H
#define PRODUCERMESSAGES_H

//Don't allow include guard for this file

#include "Macros/MessageInterfaces.h"
#include "Util/Reference.h"
#include <vector>


using namespace MsgLib;

NAMESPACE_START(Producer)
   DECLARE_MESSAGE(MsgProductsCreated, Reference<std::vector<int> >)
NAMESPACE_END(Producer)

#endif //PRODUCERMESSAGES_H
