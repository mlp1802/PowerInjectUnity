using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    public class Producer : IComparable<Producer>
    {
        public int index;
        public Invokeable invokeable;
        public object orgObj;
        public Producer(int index, object orgObj, Invokeable invokeable) {
            this.index = index;
            this.orgObj = orgObj;
            this.invokeable = invokeable;
        }
        public String getName() {
            return orgObj.GetType().FullName + ":" + invokeable.getMethodName(); 
        }
        public object invoke(object[] arguments)
        {

            return invokeable.invoke(arguments);
        }

        public int CompareTo(Producer other)
        {
            if (other == null)
            {
                return -1;
            }
            else
                if (this.lessThan(other))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
        }

        public Boolean lessThan(Producer other)
        {
            if (hasSomeOfTheSameArguments(other))
            {
                if (hasSameReturnValueAsArguments() && !other.hasSameReturnValueAsArguments())
                {
                    return true;
                }
                if (other.hasSameReturnValueAsArguments() && !hasSameReturnValueAsArguments())
                {
                    return false;
                }


            }
            return this.index < other.index;
        }

        protected Boolean hasSameReturnValueAsArguments()
        {
            var returnValue = getReturnType();
            var arguments = getArgumentTypeNames();
            return arguments.Contains(returnValue);
        }

        protected MethodInfo getFunction()
        {

            return invokeable.getMethodInfo();

        }
        protected String getReturnValue(MethodInfo m)
        {
            return m.ReturnType.FullName;
        }
        protected String getReturnType()
        {
            return getReturnValue(getFunction());
        }

        public String getReturnTypeName()
        {
            return getFunction().ReturnType.FullName;
        }

        public void printArgs()
        {
            getArgumentTypeNames().ForEach(s => Console.Write(s));
            Console.WriteLine("->" + getReturnType());
            Console.WriteLine();
        }
        public Boolean hasSomeOfTheSameArguments(Producer p)
        {
            var args1 = getArgumentTypeNames();
            var args2 = p.getArgumentTypeNames();
            var result = args1.Intersect(args2).Count() > 0;
            if (result)
            {
                var a = 23;
            }
            return result;
        }
        public List<String> getArguments(MethodInfo m)
        {
            return m.GetParameters().Select(p => p.ParameterType.FullName).ToList();
        }
        
        
        public List<String> getArgumentTypeNames()
        {

            return getArguments(getFunction());
        }
        
        public List<Parameter> getParameters() 
        {
            return ReflectionUtils.getParameters(getFunction().GetParameters().ToList());
        }
    }
    public class Parameter
    {
        public IKey key;
        public ParameterInfo info;
        public Parameter(ParameterInfo info, IKey key) 
        {
            this.info = info;
            this.key = key;
        }

    }
}


