#ifndef CXX_H
#define CXX_H

#if __cplusplus >= 201103
   #define MSG_OVERRIDE override
   #define DOWN_CAST(SubType, parentPtr) (dynamic_cast<SubType*>(parentPtr))
#else
   #define MSG_OVERRIDE
   #define DOWN_CAST(SubType, parentPtr) ((SubType*) parentPtr)
#endif

#endif // CXX_H
