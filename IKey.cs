using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerInject
{
    public interface IKey
    {
        Boolean sameAs(IKey key);
        String getCode();

    }
}
