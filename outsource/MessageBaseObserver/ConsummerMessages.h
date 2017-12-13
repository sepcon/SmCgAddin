#ifndef CONSUMMERMESSAGES_H
#define CONSUMMERMESSAGES_H
#include "Macros/MessageInterfaces.h"

NAMESPACE_START(Consummer)
DECLARE_MESSAGE(MsgHandlingProduct, int /*product*/)
DECLARE_MESSAGE(MsgListTenOfProducts, int, int, int, int, int, int, int, int, int, int)
NAMESPACE_END(Consummer)
#endif // CONSUMMERMESSAGES_H
