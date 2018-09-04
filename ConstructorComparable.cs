using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PowerInject
{
    class ConstructorComparable : IComparable<ConstructorComparable>
    {


        public ConstructorInfo info;
        public Boolean isAnnotatedWithInject = false;
        public int numberOfArguments = 0;
        public ConstructorComparable(ConstructorInfo info) 
        {
            this.info = info;
            isAnnotatedWithInject = ReflectionUtils.getAttributeInfo<Inject>(info)!=null;
            numberOfArguments = info.GetParameters().Count();

        }
        public int CompareTo(ConstructorComparable c) {

            if (this.isAnnotatedWithInject && !c.isAnnotatedWithInject)
            {
                return 1;
            }
            else {
                if (numberOfArguments > c.numberOfArguments)
                {
                    return 1;
                }
                else {
                    return -1;
                }
            }
            
        }
    }
}
