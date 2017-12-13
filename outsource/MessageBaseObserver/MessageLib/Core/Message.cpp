#include "Message.h"
#include "MessageDispatcher.h"
#include "Util/CommonLogger.h"

namespace MsgLib
{

Message::~Message()
{
}

void Message::post()
{
   MessageDispatcher::getInstance()->dispatch(this);
}

} // namespace MsgLib
