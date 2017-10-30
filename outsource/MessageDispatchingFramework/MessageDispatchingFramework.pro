TEMPLATE = app
CONFIG +=  c++11
CONFIG -= app_bundle
CONFIG -= qt
#CONFIG += console

DEFINES += ENABLE_CONSOLE_LOG

SOURCES += \
    MessageLib/Util/CommonLoger.cpp \
    MessageLib/Core/Message.cpp \
    MessageLib/Core/MessageHandler.cpp \
    MessageLib/Core/MessageDistpacher.cpp \
    Test.cpp

HEADERS += \
    MessageLib/Util/CommonLogger.h \
    MessageLib/Core/Message.h \
    MessageLib/Core/MessageHandler.h \
    MessageLib/Util/MacrosUtils.h \
    MessagesAll.h \
    MessageTypesDefines.h \
    MessageLib/Core/MessageDispatcher.h \
    MessageLib/Util/Pointer.h \
    MessageLib/MessageMacrosUtil/MsgHandlerMcrInterfaces.h \
    MessageLib/MessageMacrosUtil/MessageMcrInterfaces.h \
    MessageLib/MessageMacrosUtil/MsgMacros.h

INCLUDEPATH += MessageLib

