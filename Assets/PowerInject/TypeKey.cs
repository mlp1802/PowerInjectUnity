using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerInject
{
    public class TypeKey : IKey
    {
        public Type type;
        public TypeKey(Type t)
        {
            type = t;
        }
        public override string ToString()
        {

            return getCode();
        }

        public virtual String getCode()
        {
            return "TypeKey:" + type.FullName;
        }
        public virtual Boolean sameAs(IKey key)
        {
            if (typeof(TypeKey).IsAssignableFrom(key.GetType()))
            {
                var typeKey = (TypeKey)key;
                return typeKey.type.IsAssignableFrom(type);
            }
            else
            {
                return false;
            }
        }

    }
}
