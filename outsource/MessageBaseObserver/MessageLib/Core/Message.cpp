#include "Message.h"
#include "MessageDispatcher.h"
#include "Util/CommonLogger.h"

namespace MsgLib
{

void Message::post()
{
   MessageDispatcher::getInstance()->dispatch(this);
}

}
