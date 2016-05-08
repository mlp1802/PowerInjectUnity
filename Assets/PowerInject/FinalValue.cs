using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerInject
{
    public class FinalValue
    {
        public IKey key;
        public object obj;
        public FinalValue(IKey key, object obj)
        {
            this.key = key;
            this.obj = obj;
        }
    }
}
