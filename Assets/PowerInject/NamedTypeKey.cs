using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerInject
{
    public class NamedTypeKey : TypeKey
    {
        public String name;
        public NamedTypeKey(String name, Type t)
            : base(t)
        {
            this.name = name;
        }

        public override string getCode()
        {
            return "NamedTypeKey:" + name + ":" + base.getCode();
        }
        public override Boolean sameAs(IKey key)
        {
            if (base.sameAs(key))
            {
                if (key.GetType() == typeof(NamedTypeKey))
                {
                    var typeKeyWithName = (NamedTypeKey)key;
                    return typeKeyWithName.name == name;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

    }
}
