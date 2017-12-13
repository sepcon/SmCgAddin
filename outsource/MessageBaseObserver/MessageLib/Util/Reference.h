#ifndef REFERENCE_H
#define REFERENCE_H


namespace MsgLib {

template<typename T>
class Reference
{
public:
   Reference() {}
   Reference(T& t) : _t(t)
   {
   }
   Reference(Reference& r) : _t(r._t){}

   T& operator*()
   {
      return get();
   }

   T& get()
   {
      return _t;
   }

protected:
   T _t;
};

template<typename T>
Reference<T> makeRef(T& t)
{
   return Reference<T>(t);
}
} // namespace MsgLib

#endif // REFERENCE_H
