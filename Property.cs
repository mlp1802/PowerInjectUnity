using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    
    public class Property
    {
        public IKey key;
        public PropertyInfo propertyInfo;
        public Property(PropertyInfo p, IKey k)
        {
            propertyInfo = p;
            key = k;
        }
        public String getName()
        {
            return propertyInfo.Name;
        }

    }
}


