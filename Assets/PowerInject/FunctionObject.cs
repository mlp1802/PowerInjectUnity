using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    public class FunctionObject : Invokeable
    {
        protected MethodInfo methodInfo;
        protected object lambda;
        public FunctionObject(object lambda)
        {
            this.lambda = lambda;
            methodInfo = lambda.GetType().GetMethods().Where(a => a.Name == "Invoke").First();
        }
        public object invoke(object[] args)
        {
            return getMethodInfo().Invoke(lambda, args);
        }
        public IKey getKey() 
        {
            return null;
        }
        public MethodInfo getMethodInfo()
        {
            return methodInfo;
        }
        public String getMethodName() {
            return methodInfo.Name;
        }
    }
}

