using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PowerInject
{
    public class Field 
    {
        public IKey key;
        public FieldInfo fieldInfo;
        public Field(FieldInfo f, IKey k) 
        {
            fieldInfo = f;
            key = k;
        }
        public String getName() 
        {
            return fieldInfo.Name;
        }
    }
    
}


