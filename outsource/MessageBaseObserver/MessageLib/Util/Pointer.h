#ifndef Pointer_H
#define Pointer_H

namespace MsgLib {

template <class T>
class Pointer
{
public:
   Pointer(): _t(0){}
   Pointer(T* t) : _t(t) {}
   T* operator->()
   {
      return _t;
   }
   T& operator*()
   {
      return *_t;
   }

   bool operator!()
   {
      return !_t;
   }

   operator bool()
   {
      return _t != 0;
   }

   T* data()
   {
      return _t;
   }

private:
   T* _t;
};

}
#endif // Pointer_H
