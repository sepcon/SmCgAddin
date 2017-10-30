#ifndef MACROSUTILS_H
#define MACROSUTILS_H


/***
 * Get N'th argument passed into macro
 * */
#define _GET_NTH_ARG(_1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12 \
   , _13, _14, _15, _16, _17, _18, _19, _20, _21, N, ...) N

#define _fe_0(_call, ...)
#define _fe_1(_call,   _altercall, x)      _altercall(x, 1)
#define _fe_2(_call,   _altercall, x, ...) _call(x, 2)   _fe_1(_call,  _altercall, __VA_ARGS__)
#define _fe_3(_call,   _altercall, x, ...) _call(x, 3)   _fe_2(_call,  _altercall, __VA_ARGS__)
#define _fe_4(_call,   _altercall, x, ...) _call(x, 4)   _fe_3(_call,  _altercall, __VA_ARGS__)
#define _fe_5(_call,   _altercall, x, ...) _call(x, 5)   _fe_4(_call,  _altercall, __VA_ARGS__)
#define _fe_6(_call,   _altercall, x, ...) _call(x, 6)   _fe_5(_call,  _altercall, __VA_ARGS__)
#define _fe_7(_call,   _altercall, x, ...) _call(x, 7)   _fe_6(_call,  _altercall, __VA_ARGS__)
#define _fe_8(_call,   _altercall, x, ...) _call(x, 8)   _fe_7(_call,  _altercall, __VA_ARGS__)
#define _fe_9(_call,   _altercall, x, ...) _call(x, 9)   _fe_8(_call,  _altercall, __VA_ARGS__)
#define _fe_10(_call,  _altercall, x, ...) _call(x, 10)  _fe_9(_call,  _altercall, __VA_ARGS__)
#define _fe_11(_call,  _altercall, x, ...) _call(x, 11)  _fe_10(_call,  _altercall, __VA_ARGS__)
#define _fe_12(_call,  _altercall, x, ...) _call(x, 12)  _fe_11(_call,  _altercall, __VA_ARGS__)
#define _fe_13(_call,  _altercall, x, ...) _call(x, 13)  _fe_12(_call,  _altercall, __VA_ARGS__)
#define _fe_14(_call,  _altercall, x, ...) _call(x, 14)  _fe_13(_call,  _altercall, __VA_ARGS__)
#define _fe_15(_call,  _altercall, x, ...) _call(x, 15)  _fe_14(_call,  _altercall, __VA_ARGS__)
#define _fe_16(_call,  _altercall, x, ...) _call(x, 16)  _fe_15(_call,  _altercall, __VA_ARGS__)
#define _fe_17(_call,  _altercall, x, ...) _call(x, 17)  _fe_16(_call,  _altercall, __VA_ARGS__)
#define _fe_18(_call,  _altercall, x, ...) _call(x, 18)  _fe_17(_call,  _altercall, __VA_ARGS__)
#define _fe_19(_call,  _altercall, x, ...) _call(x, 19)  _fe_18(_call,  _altercall, __VA_ARGS__)
#define _fe_20(_call,  _altercall, x, ...) _call(x, 20)  _fe_19(_call,  _altercall, __VA_ARGS__)

#define CALL_MACRO_X_FOR_EACH(x, alterx, ...) \
    _GET_NTH_ARG("ignored", ##__VA_ARGS__, \
   _fe_20, _fe_19, _fe_18, _fe_17, _fe_16, _fe_15, _fe_14, _fe_13, _fe_12, _fe_11, \
   _fe_10, _fe_9, _fe_8, _fe_7, _fe_6, _fe_5, _fe_4, _fe_3, _fe_2, _fe_1, _fe_0)(x, alterx, ##__VA_ARGS__)


//Count the number of args passed into macro
#define __VA_NARG__(...) \
        __VA_NARG_(0, ##__VA_ARGS__, __RSEQ_N())
#define __VA_NARG_(...) \
        _GET_NTH_ARG(__VA_ARGS__)
#define __RSEQ_N() \
        20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, \
        9,  8,  7,  6,  5,  4,  3,  2,  1,  0

#if __cplusplus <= 199711L
#define DOWN_CAST(SubType, parentPtr) ((SubType*) parentPtr)
#else
#define DOWN_CAST(SubType, parentPtr) (dynamic_cast<SubType*>(parentPtr))
#endif


#endif // MACROSUTILS_H
