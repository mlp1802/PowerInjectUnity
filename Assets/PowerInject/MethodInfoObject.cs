using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    public class MethodInfoObject : Invokeable
    {
        public object obj;
        public MethodInfo info;
        IKey key;
        public MethodInfoObject(object obj, MethodInfo info,IKey key)
        {
            this.obj = obj;
            this.info = info;
            this.key = key;
        }

        public object invoke(object[] args)
        {
            return info.Invoke(obj, args);
        }
        public IKey getKey() {
            return key;
        }
        public MethodInfo getMethodInfo()
        {
            return info;
        }
        public String getMethodName()
        {
            return info.Name;
        }
    }
}