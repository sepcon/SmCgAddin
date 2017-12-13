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
    Test.cpp \
    ProducerMessages.cpp \
    AllMessages.cpp

HEADERS += \
    MessageLib/Util/CommonLogger.h \
    MessageLib/Core/Message.h \
    MessageLib/Core/MessageHandler.h \
    MessageLib/Core/MessageDispatcher.h \
    MessageLib/Util/Pointer.h \
    ProducerMessages.h \
    ConsummerMessages.h \
    MessageLib/Macros/CXX.h \
    MessageLib/Macros/MessageDefinition.h \
    MessageLib/Macros/MessageInterfaces.h \
    MessageLib/Macros/MessageHandlerInterfaces.h \
    MessageLib/Util/Macros.h \
    MessageLib/Util/Reference.h

INCLUDEPATH += MessageLib

