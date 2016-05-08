using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    public interface Invokeable
    {
        object invoke(object[] args);
        MethodInfo getMethodInfo();
        IKey getKey();
        String getMethodName();
    }
}